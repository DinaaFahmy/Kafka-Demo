Steps
*****
1-Install Conduktor. (https://www.conduktor.io)
2-Make sure that Docker is running.
3-Open power shell window in the same directory that has the "docker-compose.yml" file and type the following command:
  > docker-compose up -d
4-Now you have kafka & zookeeper services running, open Conduktor and connect to "localhost:9092" or any other servers/brokers.
5-In case of connection failure, open Docker and make sure that both kafka & zookeeper are running.

*Note: Check first that Kafka is running, and Conduktor is connected to the server you're using Before producing & consuming.