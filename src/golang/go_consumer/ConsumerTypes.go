package main

import "os"

type ConsumerStudyType int

const (
	SimpleConsume ConsumerStudyType = iota
	ConsumeWithSerialization
)

func getConsumerType() ConsumerStudyType {
	current := os.Getenv("STUDY_TYPE")
	if len(current) == 0 {
		return SimpleConsume
	}

	switch current {
	case "SimpleConsume":
		return SimpleConsume
	case "ConsumeWithSerialization":
		return ConsumeWithSerialization
	default:
		return SimpleConsume
	}
}
