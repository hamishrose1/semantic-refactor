
# Prototype Repository README

This repository contains a basic prototype for a coding agent that can iteratively plan how to complete the task it is asked to complete.
Using Plugins/Tools, the agent has access to the file system to run powershell commands and interact the file system. 

This experiment was undertaken to test whether OpenAI models are capable of upgrading an AngularJs application to new Angular. It does not seem feasible with the current models gpt4, gpt3.5.
The size of some code files means the context window of gpt4 is too small, and gpt3.5 does not seem to know how to perform the upgrade although it has a much larger context window and can handle full HTML pages.

A custom conversation service was implemented to test dolphin-mistral model using Ollama. Semantic Kernel's `FunctionCallingStepwisePlanner` iterative planner does not appear to be compatible with Ollama currently but works with the other planner types.

This worker could probably complete smaller, simpler coding jobs on its own.

## Overview

The prototype consists of a `Program` class with a `Main` method that sets up a .NET Core host. It also includes a `Worker` class that inherits from `BackgroundService`. Let's break down the main components:

1. **Program Class**:
   - The `Program` class initializes the host using `HostApplicationBuilder`.
   - It adds services, including the `Worker`, using `AddHostedService<Worker>()`. This worker executes the main logic of program.
   - Azure OpenAI integration is configured (commented out) using `AzureOpenAIChatCompletionService`.
   - A local Ollama model (also commented out) can be used as an alternative.
   - The Semantic Kernel is configured with plugins

2. **Worker Class**:
   - The `Worker` class executes background tasks.
   - It receives dependencies via constructor injection (e.g., `IHostApplicationLifetime`, `Kernel`).
   - The `ExecuteAsync` method processes a specific problem related to upgrading an AngularJS application to Angular 17.
   - A stepwise planner is used to guide the upgrade process.

3. **PowershellPlugin Class**:
   - A custom plugin allowing the agent to execute CLI commands on the host machine

4. **DirectoryIOPlugin Class**
   - A custom plugin allowing the agent to execute directory related IO commands on the host machine (similar to FileIOPlugin built in)

## Usage

1. Clone this repository.
2. Configure your Azure OpenAI credentials (if applicable).
3. Update worker prompt as needed
3. Build and run the project.
