# short-interval-scheduler-service

[![ci](https://github.com/jpfulton/short-interval-scheduler-service/actions/workflows/ci.yml/badge.svg)](https://github.com/jpfulton/short-interval-scheduler-service/actions/workflows/ci.yml)
![License](https://img.shields.io/badge/License-MIT-blue)

A short interval command scheduling Windows service implemented in .NET Core 7.

The Windows OS Task Scheduler does not allow for intervals shorter than one minute. This
service allows commands to be executed in intervals of seconds. Use cases include polling
for Azure spot instance eviction events and triggering a graceful shutdown via a powershell
script.
