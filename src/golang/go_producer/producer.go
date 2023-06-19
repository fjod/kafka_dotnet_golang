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

	fmt.Fprintf(os.Stdout, "Producer is starting..\n")
	studyType := getProducerType()
	switch studyType {
	case SimpleProduce:
		fmt.Fprintf(os.Stdout, "SimpleProduce\n")
		performSimpleProduce(conf)
	case ProduceWithSerialization:
		fmt.Fprintf(os.Stdout, "ProduceWithSerialization\n")
		performProduceWithSerialization(conf, os.Args[2])
	}
}
