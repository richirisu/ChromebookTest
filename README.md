# ChromebookTest

Test application for demonstrating issues with Xamarin applications running on Chromebooks

## About the issue

First of all, this issue *only occurs* on Chromebooks running Xamarin Android applications. It does *not occur* on Android phones when running the same application.

When using asynchronous tasks, it happens every so often that the tasks will not return, *unless* the user actively taps or clicks the screen. Depending on the amount of asynchronous tasks this may require multiple taps to finish.

Although more cumbersome, besides tapping the screen, it is also possible to force the tasks to eventually return by minimizing and maximizing the application.

Since tapping the screen multiple times leads to the tasks returning eventually, a typical deadlock situation can be eliminated.

## How to reproduce

In order to reproduce this issue, you have to have some sort of *nested* tasks. Normally, the nested tasks should be executed one by one. However, the application keeps stopping the execution of the nested tasks from time to time. Strangely, by tapping on the display, the application continues to execute the next few nested tasks until it stops again.

```
await Task.Run(async () -> {
   await Task.Run(...);
   await Task.Run(...);
   ...
});
```

This works for both Task.Run() and TaskScheduler.StartNew() alike. (Most likely because the task scheduler internally makes a call to Task.Run() itself.)

```
await MyTaskScheduler.StartNew(async () -> {
   await Task.Run(...);
   await Task.Run(...);
   ...
});
```

## What devices are affected

Chromebooks running Xamarin Android applications

→ chrome://system

| Model | CPU | Chrome OS | Android SDK |
| --- | --- | --- | --- |
| Asus Chromebook Flip C434T | Intel Core m3 8th Gen (X64) | 102 | 28 |

## Regarding this test application

This test application is made to be as simple as possible in order to reproduce the issue. It has a single page with a button that uppon calling makes some asynchronous (super simple) calculations. When clicking the button, a single new Task will be dispatched which executes a given number of nested tasks in order, e.g. 100 nested tasks. See the application log for the current number of executed tasks. When all nested tasks have been executed, the parent task returns and an alert is beeing showed.

You can click the button again to start over.

Note, that you can cancel the process by clicking the button again during calculation.

The problem is, that not all nested tasks are being executed as expected. Moreover, the user has to actively tap on the screen to force the continuation of the task execution. You can confirm this with the application logging output.
