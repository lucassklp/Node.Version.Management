# Node.Version.Management

A multi-plataform node version management tool.

Note: This project is under development. The current version is just a Proof of Concept and is working only on windows, but a stable version is coming.

## Requirements

You need .NET Core 3 installed to run this application.

## Installation

```bash
dotnet tool install -g Node.Version.Management
```

Note: Installation not working yet (package not published). Will be working soon.

## Running

Just use following command on bash:

```bash
Node.Version.Management
```

## Commands

Use the latest version:

```bash
Node.Version.Management --use latest
```

Or specify a version:

```bash
Node.Version.Management --use 10.18.0
```

List available local versions:

```bash
Node.Version.Management --list
```



Note: Commands are under development. This is how the tool will be working on stable version.