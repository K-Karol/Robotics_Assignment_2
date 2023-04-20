# Karol Kierzkowski's Robotics Assignment 2 - 29018532

Welcome to the repository containing the source code the second SHU Robotics assignment.

Video Links:

* Personal Google Drive: <https://drive.google.com/file/d/11D6XcvKT1-S02CLfEpxCy4ayH5hUoE-P/view?usp=sharing>
* Network One Drive (Requires login): <https://sheffieldhallam-my.sharepoint.com/:v:/g/personal/b9018532_hallam_shu_ac_uk/EcJSjUiOpWZPqcfYhNvBb3IBVMpJ6L2-QPiDyLakUB9uGA?e=umS7IG>

The repository is split into 2 sections:

* **ev3_firmware**: Contains all of the MicroPython files that are deployed to the Lego Mindstorms EV3 hub.
* **ev3_software**: A Microsoft Visual Studio 2022 Solution containing .NET projects that are used in the creation of a client application using .NET MAUI

## Building

### ev3_firmware

The **ev3_firmware** folder contains several MicroPython files, with `main.py` being the main entry point that is executed by EV3. Since Python is intepreted, no building is necessary.

To deploy the project to the EV3, the EV3 must be flashed with `ev3dev`, a Debian Linux-based operating system that runs on the EV3 through the use of a flashed SD Card. The image can be found on [the official LEGO website](https://education.lego.com/en-us/product-resources/mindstorms-ev3/teacher-resources/python-for-ev3), and optionally from [the ev3dev website](https://www.ev3dev.org/), although I've used the image from the LEGO website.

Once flashed, the EV3 needs to be connected to your machine which can be done through either a USB cable (easiest), or Bluetooth. Refer to the [guide](https://pybricks.com/ev3-micropython/index.html) on how to setup an environment for developing with `ev3dev`.

Deploying is taken care of when running the `Download and Run` configuration in VSCode created by the [LEGO® MINDSTORMS® EV3 MicroPython extension](https://marketplace.visualstudio.com/items?itemName=lego-education.ev3-micropython) which deploys all of the files and configures them so they are ran properly. Manually deploying the files requires an extra step of configuring the permission for the entry `.py` script as explained in [this page in the guide](http://docs.ev3dev.org/en/ev3dev-stretch/programming/fundamentals.html).

### ev3_software

The **ev3_software** folder contains a *Visual Studio Solution* built using Visual Studio 2022 that describes 3 projects:

* **KarolK72.LegoAssignment.Library**: This is a class library that contains classes and interfaces that are used to develop the application but are not specific to the UI project and can be reused in other projects.
* **KarolK72.LegoAssignment.UI**: This is a .NET MAUI project that contains a client application that communicates with the EV3 to provide a dashboard and allow for the configuration of the firmware.
* **KarolK72.LegoAssignment.Tests**: This is a unit testing project (MSTest) that allows for creating tests to ensure certain functionaility behaves as expected, such as whether the `Payload` class correctly parses data.

The projects are built using Visual Studio 2022. The **KarolK72.LegoAssignment.UI** does require Visual Studio to be setup to allow for the creation of .NET MAUI applications, including downloading and installing the Android SDK.

The **KarolK72.LegoAssignment.UI** project is cross-platform, therefore it can run on a range of platform; however, only *Windows* and *Android* was tested but it should run in MacOS without much issues (there might be visual changes).

You may run the UI application using Visual Studio, or you may build and publish the application using Visual Studio [using this guide](https://learn.microsoft.com/en-us/dotnet/maui/deployment/?view=net-maui-7.0) which shows all of the steps of publishing an application for each platform (including using the CLI).

#### Publishing to Windows using Visual Studio

* Right-Click on the **KarolK72.LegoAssignment.UI** project and select *Publish*
* Select *Sideloading* and *Next*.
* For the signing method, under the `Yes, select a certificate`, press `Create` and create a self-signed certificate (you don't need a password). Once created, you need to trust the certificate. Once trusted, press *Next*.
* In *Publishing Profile*, create a new profile if there is no profile to use using the following configurations:
  * Configuration: Select *Release*
  * Target Runtime: Select *win10-x64*
  * Self-Contained: *True*
* Press *Create*.
* Visual Studio will now create and application, and once it is finished a menu pops up with the output location which you can navigate to.
* Once in the published application folder, run the `*.msix` file that is prefixed with `KarolK72.LegoAssignment.UI` following by the version number.
* An *App Installer* menu pops up, install the application and it is now installed and available using the Windows search.

## Usage

Once the **ev3_firmware** files are deployed (`Download and Run` VSCode launch configuration was ran at least once, or the files were manually copied), the firmware can be launched by using the `Brickman` UI on the EV3 (running `ev3dev`), navigating the the **File Browser**, entering into the folder that contains the firmware (*usually `ev3_firmware`) and launching the `main.py` file which should be decorated with an `*` character (`main.py*`). If the `*` character is not there, then the permissions for the file were not set.

Once the firmware is running, the EV3 screen should display a `Connecting...` message on the first line and shortly after `@ 5000` on the second line. The EV3 is now waiting for the client application to initialise a connection.

### Initialising a connection with the EV3 brick

Prior to running the firmware, the EV3 brick needs to be connected through either a USB cable or Bluetooth and IPv4 needs to be configured. The easiest method is using Bluetooth which is what I will describe, if you need to use the USB cable, refer to [this guide](https://www.ev3dev.org/docs/tutorials/connecting-to-the-internet-via-usb/).

Navigate **Wireless and Networks** root menu, **Bluetooth** and if you haven't paired your device with the EV3 brick yet, then pair your device by either scanning on the EV3 and selecting your device from the list (given your device is visible), or selecting the `Visible` checkbox on the EV3 and connecting from your device.

Once the device has been paired, in the device menu (found by clicking on the device name in the **Bluetooth** menu. If you just paired the device, you will be in that menu) there is a `Network Connection` button that takes you to another menu.

Press **Connect** and allow the device to connect. Even if there is a timeout, it still might be connecting. Once the status changes to **Connected** or **Online**, the device is now connected to your device's network. An IP address should appear at the top of the EV3 status bar.

The IP address is used to connect to the EV3 brick. On Windows you may use the DNS name `ev3dev.local`, but on Android for example that does not work and you need to use the IP address.

### Connecting the client application to the EV3 brick

Once you have an IPv4 connection and the firmware running, the brick will speak and say *Waiting for connection*. If there is no spoken error message within 5 seconds, you may connect. The screen shows a `@ <PORT>` message with `<PORT>` representing the port number, which should be 5000.

To connect, launch the software. The first panel contains a `Host` and `Port` fields, modify the `Host` to the IP number that was displayed on the top of the EV3 screen if you cannot connect using `ev3dev.local` hostname (Android does not work). The application will connect and the status will change to `Connected` status and the robot will start operating (the conveyour belt will start to move).

### Operation

Once connected, the robot will start the conveyour belt and it will start to operate. Each coloured piece of lego represents a vegetable that the robot will either allow to pass through, or will reject and push it off the conveyour belt.

The default configuration is for *fresh tomatoes*, where it only accepts red *tomatoes* (lego bricks) while rejecting everything else.

Each time a vegetable is scanned, the middle/second panel will update with the newest statistic including the number of processed items (*vegetables*), the number of rejected items and a rejection rate percentage.

### Configuration

The application allows for the configuration of the robot's rejection rules. Pressing the *Fetch newest config* button will request the configuration details from the robot so it can show you the current configuration in the UI.

To edit the configuration, make use of the following UI elements and once finished changing the configuration, use the *Update configuration* button to send the new configuration to the robot and changes are effective immediately:

#### Is Blacklist?

This toggle determines whether the following list of colours should be a *whitelist* or a *blacklist*. If checked, then it is a *blacklist* otherwise it will be a *whitelist*

#### List of Colours

This selection list allows the use to select multiple colours. Depending on whether *Is Blacklist?* is checked or not, this list will either be a *whitelist* (Only the colours in the list are permitted), or a *blacklist* (The colours on the list are rejected).

### Stopping

During operation, the middle button on the EV3 can be used to gracefully stop the operation. Otherwise, you can use the EV3 back button to force close of the application.

## References

* .NET MAUI Documentation: <https://learn.microsoft.com/en-us/dotnet/maui/?view=net-maui-7.0>
* Socket Programming in .NET/C#:
  * <https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services>
  * <https://www.geeksforgeeks.org/socket-programming-in-c-sharp/>
  * <https://www.c-sharpcorner.com/article/socket-programming-in-C-Sharp/>
* Socket Programming in Python: <https://realpython.com/python-sockets/>
* PyBricks EV3 MicroPython documentation: <https://pybricks.com/ev3-micropython/>
* Python/MicroPython multithreading:
  * <https://docs.python.org/3.5/library/threading.html>
  * <https://mpython.readthedocs.io/en/master/library/micropython/_thread.html>
  * <https://www.electrosoftcloud.com/en/multithreaded-script-on-raspberry-pi-pico-and-micropython/>
* Payload format inspired by the payload format used in my previous project <https://github.com/K-Karol/Robotics_Assignment>
