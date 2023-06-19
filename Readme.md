# Hello-world for kafka using dotnet and golang


To start, open in terminal and enter`docker-compose up -d` in the top directory of the repo. There will be two kafka clusters,
two consumers and two producers. There will be sample 3 topics created.

Producers and consumers have different modes. Check src/dotnet/Common/StudyTypes enums. 
To choose mode, simply change its value in `docker-compose.yml`. 
All modes work with first cluster kafka0 (29092 inside docker, 9092 for localhost).

For golang, github.com/confluentinc/confluent-kafka-go/kafka does not work on windows:
[1](https://github.com/confluentinc/confluent-kafka-go/issues/889), [2](https://github.com/confluentinc/confluent-kafka-go/issues/128).

I followed [this](https://www.jetbrains.com/help/go/how-to-use-wsl-development-environment-in-product.html#create_project_for_wsl) guide 
and all worked pretty fine in wsl. Dockerfile for building kafka with golang is from [here](https://github.com/confluentinc/confluent-kafka-go/issues/461).

Modes are changed in docker-compose.yml via env variables. Check values of these variables in corresponding enums.
## Brief explanation of modes in dotnet part of codebase:

### Producer:
* SimpleProduce - produce 10 messages to default topic in kafka0 and exit
* WithNewTopic - starts new topic and produce 10 messages to it
* WithCustomPartitioner - uses custom partitioner to evenly divide all sample users into 2 partitions
* SimpleProduceWithParameters - produce `X amount` batches of messages with `Y delay` between sending
* ProduceWithSerializer - produce some messages of `Dummy` type to `dummyTopic` using protobuf serialization

### Consumer:
* SimpleConsume - consume from default topic in kafka0
* UsingDataflow - consume from default topic in kafka0 using dataflow nuget
* ConsumeFromTopic - consume from partition 0 in default topic in kafka0
* ConsumeWithSerialization - consume messages from `dummyTopic` and deserialize them using protobuf

Note, that all producers exit after finishing the job. All consumers spin forever.

## Brief explanation of modes in golang part of codebase:

### Producer:
* SimpleProduce - produce many messages to default topic in kafka0 and exit
* ProduceWithSerializer - produce some messages of `Dummy` type to `dummyTopic` using protobuf serialization

### Consumer:
* SimpleConsume - consume from default topic in kafka0
* ConsumeWithSerialization - consume messages from `dummyTopic` and deserialize them using protobuf

golang protobuf modes need connection to schemaregistry, the url is passed via command line args. Check dockerfiles. You might need to restart schemaregistry after fresh `docker-compose up`.
## Misc

* It's not necessary to restart all docker containers if you want to check another mode.
Use `docker-compose up -d --build consumer`to rebuild & start only consumer container.
You can rebuild container from docker compose from scratch using `--no-cache` option, for example
  `docker-compose build consumer --no-cache`

* After start of all containers, there is ~30 seconds delay on my PC 
for kafka clusters to become operational. 

* Original docker-compose is from [kafka-ui](https://github.com/provectus/kafka-ui/blob/master/documentation/compose/kafka-ui.yaml).
To observe topics and messages, visit http://localhost:8080/ when containers are up and running, though it often shows 404 error instead of topic' messages.

* Message has random key and value (selected from perspective hardcoded arrays of 6 samples).

* If you want to debug code, change `bootstrap.servers=kafka0:29092`  to `bootstrap.servers=localhost:9092` in .properties file.

* In this [gist](https://gist.github.com/miguelmota/25568433ad8cfddb5ea556a5644c9fde) there is an golang example how to make object from byte array and vice versa, if there is a proto file. So using ser/deser as written in corresponding golang protobuf files is not needed, you can create byte array by yourself, just use the type from proto file:

``` 
 var text = []byte("hello") 
 message := &example.Message{
 Text: text,
 }

 data, err := proto.Marshal(message)
 if err != nil {
 panic(err)
 }

 fmt.Println(data) // [10 5 104 101 108 108 111]

 newMessage := &example.Message{}
 err = proto.Unmarshal(data, newMessage)
 if err != nil {
 panic(err)
 }

 fmt.Println(newMessage.GetText()) // [104 101 108 108 111]

```
* Code in this repo is for educational purposes only. **I have no idea what I'm doing.**