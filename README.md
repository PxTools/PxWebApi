# PxWeb
This is the official source code repository for PxWeb. PxWeb is a web application for dissemination of statistical tables please read more abou it at the official web page on Statistics Sweden web site at [www.scb.se/px-web](https://www.scb.se/px-web).

## Current activities
We are currently porting the core part of PxWeb, which we call Px framework to .NET Standard. At the same time, we split up this core parts to individual nuget packages. Thereby making them more reusable for other applications. This is the first step of what we call PxWeb NextGen where we will rewrite the UI using new technologies and leaving the old ASP.NET Web Forms behind.
Please head over to the netstandard branch to see the activities.

## Compiling the source
To be able to compile the master branch you have to generate your own set of signing keys and add them to the relevant places. If you do not want to bother it that we suggest that you try out the netstandard branch where we have removed the signing of the assemblies.

## Installation
If you are looking for a compiled version of the source, ready to install in your environment. Then please send a mail to [pc-axis@scb.se](mailto:pc-axis@scb.se?subject=Access%20to%20download%20portal) and you will receive credentials to Statistics Sweden’s download portal where you can download a complied version together with instructions.
