# event-store

Event Store DB event sourcing sample


## Installation

1- First, pull esdb (event sourcing database) docker image from docker hub:
```bash
  docker pull eventstore/eventstore
```

2- Second, create and run its docker containter in 'Insecure' mode:
```bash
  docker run --name esdb-node -it -d -p 2113:2113 -p 1113:1113 eventstore/eventstore:latest --insecure --run-projections all --startstandardprojections --enable-external-tcp --enable-atom-pub-over-http
```

3- Set 'Api' project as 'Startup Project'

4- Run the 'Api'

### Some references


- [Event Store website](https://www.eventstore.com/)

- [Docker Hub](https://hub.docker.com/r/eventstore/eventstore)
