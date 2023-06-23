package main

import (
	"fmt"
	"github.com/confluentinc/confluent-kafka-go/kafka"
	"math/rand"
	"os"
	"time"
)

func performSimpleProduce(conf kafka.ConfigMap) {
	topic := "first.messages"
	p, err := kafka.NewProducer(&conf)

	if err != nil {
		fmt.Printf("Failed to create producer: %s", err)
		os.Exit(1)
	}

	// Go-routine to handle message delivery reports and
	// possibly other event types (errors, stats, etc)
	// implemented as deliveryChan, but this way also works
	/*go func() {
		for e := range p.Events() {
			switch ev := e.(type) {
			case *kafka.Message:
				if ev.TopicPartition.Error != nil {
					fmt.Printf("Failed to deliver message: %v\n", ev.TopicPartition)
				} else {
					fmt.Printf("Produced event to topic %s: key = %-10s value = %s\n",
						*ev.TopicPartition.Topic, string(ev.Key), string(ev.Value))
				}
			}
		}
	}()*/

	users := [...]string{"eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther"}
	items := [...]string{"book", "alarm clock", "t-shirts", "gift card", "batteries"}

	deliveryChan := make(chan kafka.Event)

	for i := 0; i < 10000; i++ {
		time.Sleep(100 * time.Millisecond)

		for n := 0; n < 1000; n++ {
			key := users[rand.Intn(len(users))]
			data := items[rand.Intn(len(items))]
			p.Produce(&kafka.Message{
				TopicPartition: kafka.TopicPartition{Topic: &topic, Partition: kafka.PartitionAny}, // change Partition to 0 or 1 if you want to send messages there
				Key:            []byte(key),
				Value:          []byte(data),
			}, deliveryChan)

			e, ok := <-deliveryChan
			if !ok {
				fmt.Printf("Channel is closed for kafka producer")
			}
			m := e.(*kafka.Message)
			fmt.Printf("Message is produced to topic %s with partition %d: key = %-10s value = %s\n",
				*m.TopicPartition.Topic,
				m.TopicPartition.Partition,
				string(m.Key),
				m.Value)
		}
	}

	// Wait for all messages to be delivered
	p.Flush(15 * 1000)
	p.Close()
}
