# Description

This repo contains .NET console programs for testing [Rebus](https://github.com/rebus-org/Rebus) performance.

There are two Rebus clients - a publisher and a subscriber that both connect to the same Rebus over a RabbitMQ message bus.

Each program can be started in multiple instances.

The exchanged messages and the corresponding transmission delays are logged to the console.

### Usage

The test is running in docker containers using docker-compose.

Build the docker images for the publisher and the subscriber by running **build-docker-images.bat**

Then run the test using this command

    docker-compose up

When the test has completed abort docker-compse by pressing CTRL-C.<br />
Then run this command to cleanup:

    docker-compose down

### Observations

Delivering a message to the bus (*bus.Publish()*) seems to be where the delay occurs.

Changing SetNumberOfWorkers and SetMaxParallelism to other values than default (1 and 5) has no significant effect.

Changing SetBackoffTimes has no significant effect.

Adding the Rebus header **Header.Express = ""** has no significant effect.

### Rebus Github Issues

This Rebus issue is similar to what we experience:
https://github.com/rebus-org/Rebus/issues/436