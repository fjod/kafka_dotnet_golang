package main

import (
	"fmt"
	"github.com/confluentinc/confluent-kafka-go/kafka"
	"github.com/confluentinc/confluent-kafka-go/schemaregistry"
	"github.com/confluentinc/confluent-kafka-go/schemaregistry/serde"
	"github.com/confluentinc/confluent-kafka-go/schemaregistry/serde/protobuf"
	"github.com/golang/protobuf/proto"
	"os"
	"os/signal"
	"syscall"
)

func performConsumeWithSerialization(conf kafka.ConfigMap, schemaRegistryUrl string) {
	sigchan := make(chan os.Signal, 1)
	signal.Notify(sigchan, syscall.SIGINT, syscall.SIGTERM)

	consumer, err := kafka.NewConsumer(&conf)

	if err != nil {
		fmt.Fprintf(os.Stderr, "Failed to create consumer: %s\n", err)
		os.Exit(1)
	}

	topic := "dummyTopic"

	client, err := schemaregistry.NewClient(schemaregistry.NewConfig(schemaRegistryUrl))

	if err != nil {
		fmt.Printf("Failed to create schema registry client: %s\n", err)
		os.Exit(1)
	}

	deser, err := protobuf.NewDeserializer(client, serde.ValueSerde, protobuf.NewDeserializerConfig())

	if err != nil {
		fmt.Printf("Failed to create deserializer: %s\n", err)
		os.Exit(1)
	}

	deser.ProtoRegistry.RegisterMessage((&Dummy{}).ProtoReflect().Type())

	err = consumer.SubscribeTopics([]string{topic}, nil)

	run := true

	for run {
		select {
		case sig := <-sigchan:
			fmt.Printf("Caught signal %v: terminating\n", sig)
			run = false
		default:
			ev := consumer.Poll(100)
			if ev == nil {
				continue
			}

			switch e := ev.(type) {
			case *kafka.Message:
				for i := 0; i < len(e.Value); i++ {
					fmt.Printf("%d ", e.Value[i])
				}
				magic := []byte{0, 0, 0, 0, 1, 0} // first 6 symbols are 0 0 0 0 1 0  for some reason only in golang :shrug:
				conv := append(magic, e.Value...)
				value, err := deser.Deserialize(*e.TopicPartition.Topic, conv)
				if err != nil {
					fmt.Printf("Failed to deserialize payload: %s\n", err)
				} else {
					fmt.Printf("%% Message on %s:\n%+v\n", e.TopicPartition, value)
				}
				if e.Headers != nil {
					fmt.Printf("%% Headers: %v\n", e.Headers)
				}
				dummy := &Dummy{}
				err = proto.Unmarshal(e.Value, dummy) // but here we don't need to apply these 6 first symbols
				if err != nil {
					fmt.Printf("failed to unmarshal: %v\n", err)
				} else {
					fmt.Printf("%% Name: %s, Age: %d\n", dummy.Name, dummy.Age)
				}
			case kafka.Error:
				// Errors should generally be considered
				// informational, the client will try to
				// automatically recover.
				fmt.Fprintf(os.Stderr, "%% Error: %v: %v\n", e.Code(), e)
			default:
				fmt.Printf("Ignored %v\n", e)
			}
		}
	}

	fmt.Printf("Closing consumer\n")
	consumer.Close()
}
