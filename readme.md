![](/docs/images/events-and-services.png)

## Introduction

`ServiceExplorer` is a command line tool to help developers get information about our microservices and their relationship through integration events rendered as dependency graph. 

## Basic usage

### Help

With the tool installed, type:

```shell
ServiceExplorer.CommandLine --help
```

You should see some information about the tool, like its current installed version and available commands.

### Configuration

To start using the tool you need to clone the Mapiq-One repository to your machine. With the repository cloned, for instance inside the folder `c:\Mapiq`, run the commands below:

```shell
ServiceExplorer.CommandLine config --add-folder C:\Mapiq\Mapiq-One\
```

### Sample comand (output to terminal)

Now type the command below:

```shell
ServiceExplorer.CommandLine service --show-raising-event --show-listening-event --ignore-service-without-event --ignore-not-listened-event
```

You should see a list of services and their integration events (raising and listening) being listed on the terminal.

### Sample comamnd (output to a chart)

Now type the same command you've used above, but just add the option `--plot`:

```shell
ServiceExplorer.CommandLine service --show-raising-event --show-listening-event --ignore-service-without-event --ignore-not-listened-event --plot
```

You should see an image (SVG) opened on your browser showing all services and events that are raised and listened.

For more detailed information about the commands available, read the next sections.

## ⚙️ Config command

### Help

```shell
ServiceExplorer.CommandLine config --help
```

### Adding folders

Before starting to use the `ServiceExplorer`, we need to add the folder for `Mapiq-One`.
If you don't have these repositories cloned in your machine, please clone them now.

In this example, I'm assuming that you've cloned the two folders inside the folder `c:\Mapiq`.

```shell
ServiceExplorer.CommandLine config --add-folder C:\Mapiq\Mapiq-One\
```

### Removing folders

If you've added a wrong folder, you can use the command bellow to remove it from the configuration.

```shell
ServiceExplorer.CommandLine.exe config --remove-folder C:\Mapiq\Wrong-Folder\
```

## 📦 Service command

### Help

```shell
ServiceExplorer.CommandLine.exe service --help
```

### Show all services

```shell
ServiceExplorer.CommandLine.exe service
```

### Show all services and events raised by them

```shell
ServiceExplorer.CommandLine.exe service --show-raising-event
```

### Show all services and events listened by them

```shell
ServiceExplorer.CommandLine.exe service --show-listening-event
```

### Show all services and events raised and listened by them

```shell
ServiceExplorer.CommandLine.exe service --show-raising-event --show-listening-event
```

### Show all services and events raised and listened by them, ignoring services without event and events that are not listened by any other service

```shell
ServiceExplorer.CommandLine.exe service --show-raising-event --show-listening-event --ignore-service-without-event --ignore-not-listened-event
```

### Plotting

For any combination of arguments/options used with the `Service command` you can use the option `--plot`, then instead of writing the output to a terminal a chart will be plotted.

If you run the previous command, but add the `--plot` option, a chart like this will be generated:

```shell
ServiceExplorer.CommandLine.exe service --show-raising-event --show-listening-event --ignore-service-without-event --ignore-not-listened-event --plot
```

![](/docs/images/services-and-events.png)

### Filtering services

You can filter the services that will appear in the result by using the `--filter-service` option.
This option can be used multiple times to filter multiple services.

Using the same command used in the Plotting section but adding a filter for Office Shifts service and Building service you will have a chart showing only the services and events of that context and you can easly see how is the relationship of these services in regard to integration events.

```shell
ServiceExplorer.CommandLine.exe service --show-raising-event --show-listening-event --ignore-service-without-event --ignore-not-listened-event --filter-service shifts --filter-service building --plot
```

![](/docs/images/services-and-events-filtered.png)

## ↔️ Event command

```shell
ServiceExplorer.CommandLine.exe event --help
```

### Show all events

```shell
ServiceExplorer.CommandLine.exe event
```

### Show all events and services that raised them

```shell
ServiceExplorer.CommandLine.exe event --show-service-raising
```

### Show all events and services that listened them

```shell
ServiceExplorer.CommandLine.exe event --show-service-listening
```

### Show all events and services that raised and listened them

```shell
ServiceExplorer.CommandLine.exe event --show-service-raising --show-service-listening
```

### Show all events and services that raised and listened them, ignoring not listened events

```shell
ServiceExplorer.CommandLine.exe event --show-service-raising --show-service-listening --ignore-not-listened-event
```

### Plotting

For any combination of arguments/options used with the `Event command` you can use the option `--plot`, then instead of writing the output to a terminal a chart will be plotted.

If you run the previous command, but add the `--plot` option, a chart like this will be generated:

```shell
ServiceExplorer.CommandLine.exe event --show-service-raising --show-service-listening --ignore-not-listened-event --plot
```

![](/docs/images/events-and-services.png)

### Filtering events

You can filter the events that will appear in the result by using the `--filter-event` option.
This option can be used multiple times to filter multiple events.

Using the same command used in the Plotting section but adding filter for BuildingDeleted and BuildingPublished events you will have a chart showing only the events and services of that context and you can easily see how is the relationship of these events with the services.

```shell
ServiceExplorer.CommandLine.exe event --show-service-raising --show-service-listening --ignore-not-listened-event --filter-event BuildingDeleted --filter-event BuildingPublished --plot
```

![](/docs/images/events-and-services-filtered.png)