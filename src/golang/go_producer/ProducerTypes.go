package main

import (
	"fmt"
	"os"
)

type ProducerStudyType int

const (
	SimpleProduce ProducerStudyType = iota
	ProduceWithSerialization
)

func getProducerType() ProducerStudyType {
	current := os.Getenv("STUDY_TYPE")
	fmt.Fprintf(os.Stdout, "found study type: %s\n", current)
	if len(current) == 0 {
		return SimpleProduce
	}

	switch current {
	case "SimpleProduce":
		return SimpleProduce
	case "ProduceWithSerialization":
		return ProduceWithSerialization
	default:
		return SimpleProduce
	}
}
