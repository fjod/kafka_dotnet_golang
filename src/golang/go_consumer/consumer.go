package main

import (
	"fmt"
	"os"
)

func main() {

	if len(os.Args) < 2 {
		fmt.Fprintf(os.Stderr, "Usage: %s <config-file-path> %s <schemaregistry0-url>\n",
			os.Args[1], os.Args[2])
		os.Exit(1)
	}
	configFile := os.Args[1]
	conf := ReadConfig(configFile)
	conf["group.id"] = "kafkaStudy" // same as C# consumer
	conf["auto.offset.reset"] = "earliest"

	fmt.Fprintf(os.Stdout, "Consumer is starting..\n")
	studyType := getConsumerType()
	switch studyType {
	case SimpleConsume:
		fmt.Fprintf(os.Stdout, "SimpleConsume\n")
		performSimpleConsume(conf)
	case ConsumeWithSerialization:
		fmt.Fprintf(os.Stdout, "ConsumeWithSerialization\n")
		performConsumeWithSerialization(conf, os.Args[2])
	}
}
