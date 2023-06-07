# Hello-world for kafka using dotnet and golang


To start, open in terminal and enter`docker-compose up -d` in the top directory of the repo. There will be two kafka clusters,
two consumers and two producers. There will be sample 3 topics created.

Producers and consumers have different modes. Check src/dotnet/Common/StudyTypes enums. 
To choose mode, simply change it's value in `docker-compose.yml`. 
All modes work with first cluster kafka0 (29092 inside docker, 9092 for localhost).

For golang, github.com/confluentinc/confluent-kafka-go/kafka does not work on windows:
https://github.com/confluentinc/confluent-kafka-go/issues/889
https://github.com/confluentinc/confluent-kafka-go/issues/128

I followed [this](https://www.jetbrains.com/help/go/how-to-use-wsl-development-environment-in-product.html#create_project_for_wsl) guide 
and all worked pretty fine in wsl.

## Brief explanation of modes:

### Producer:
* SimpleProduce - produce 10 messages to default topic in kafka0 and exit
* WithNewTopic - starts new topic and produce 10 messages to it
* WithCustomPartitioner - uses custom partitioner to evenly divide all sample users into 2 partitions
* SimpleProduceWithParameters - produce `X amount` batches of messages with `Y delay` between sending

### Consumer:
* SimpleConsume - consume from default topic in kafka0
* UsingDataflow - consume from default topic in kafka0 using dataflow nuget
* ConsumeFromTopic - consume from partition 0 in default topic in kafka0

Note, that all producers exit after finishing the job. All consumers spin forever.

## Misc

* It's not necessary to restart all docker containers if you want to check another mode.
Use `docker-compose up -d --build consumer`to rebuild & start only consumer container.

* After start of all containers, there is ~30 seconds delay on my PC 
for kafka clusters to become operational. 

* Original docker-compose is from [kafka-ui](https://github.com/provectus/kafka-ui/blob/master/documentation/compose/kafka-ui.yaml).
To observe topics and messages, visit http://localhost:8080/ when containers are up and running.

* Message has random key and value (selected from perspective hardcoded arrays of 6 samples).

* If you want to debug code, change `bootstrap.servers=kafka0:29092`  to `bootstrap.servers=localhost:9092` in .properties file.