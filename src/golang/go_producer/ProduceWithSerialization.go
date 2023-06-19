package main

import (
	"fmt"
	"github.com/confluentinc/confluent-kafka-go/kafka"
	"github.com/confluentinc/confluent-kafka-go/schemaregistry"
	"github.com/confluentinc/confluent-kafka-go/schemaregistry/serde"
	"github.com/confluentinc/confluent-kafka-go/schemaregistry/serde/protobuf"
	"math/rand"
	"os"
)

func performProduceWithSerialization(conf kafka.ConfigMap, schemaRegistryUrl string) {
	topic := "dummyTopic"
	p, err := kafka.NewProducer(&conf)
	if err != nil {
		fmt.Printf("Failed to create producer: %s\n", err)
		os.Exit(1)
	}

	client, err := schemaregistry.NewClient(schemaregistry.NewConfig(schemaRegistryUrl))

	if err != nil {
		fmt.Printf("Failed to create schema registry client: %s\n", err)
		os.Exit(1)
	}

	ser, err := protobuf.NewSerializer(client, serde.ValueSerde, protobuf.NewSerializerConfig())

	if err != nil {
		fmt.Printf("Failed to create serializer: %s\n", err)
		os.Exit(1)
	}

	users := [...]string{"eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther"}
	deliveryChan := make(chan kafka.Event)

	for i := 0; i < 10; i++ {
		value := Dummy{
			Name: users[rand.Intn(len(users))],
			Age:  int32(i),
		}
		payload, err := ser.Serialize(topic, &value)
		if err != nil {
			fmt.Printf("Failed to serialize payload: %s\n", err)
			os.Exit(1)
		}

		payload = payload[6:] // first 6 symbols are 0 0 0 0 1 0  for some reason only in golang :shrug:
		err = p.Produce(&kafka.Message{
			TopicPartition: kafka.TopicPartition{Topic: &topic, Partition: kafka.PartitionAny},
			Value:          payload,
			Headers:        []kafka.Header{{Key: "go_producer", Value: []byte("header values are binary")}},
			Key:            []byte(value.Name),
		}, deliveryChan)
		if err != nil {
			fmt.Printf("Produce failed: %v\n", err)
			os.Exit(1)
		}
		e := <-deliveryChan
		m := e.(*kafka.Message)
		if m.TopicPartition.Error != nil {
			fmt.Printf("Delivery failed: %v\n", m.TopicPartition.Error)
		} else {
			fmt.Printf("Delivered message to topic %s [%d] at offset %v\n",
				*m.TopicPartition.Topic, m.TopicPartition.Partition, m.TopicPartition.Offset)
			fmt.Printf("payload: %s %d, len %d \n", value.Name, value.Age, len(payload))
			for i := 0; i < len(payload); i++ {
				fmt.Printf("%d ", payload[i])
			}
		}
	}

	close(deliveryChan)
}
