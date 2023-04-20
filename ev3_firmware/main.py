#!/usr/bin/env pybricks-micropython
from pybricks.hubs import EV3Brick
from pybricks.ev3devices import (Motor, TouchSensor, ColorSensor,
                                 InfraredSensor, UltrasonicSensor, GyroSensor)
from pybricks.parameters import Port, Stop, Direction, Button, Color
from pybricks.tools import wait, StopWatch, DataLog
from pybricks.robotics import DriveBase
from pybricks.media.ev3dev import SoundFile, ImageFile
import time

from socket_connection import SocketConnection
from configuration import Configuration
from payload import Payload

import sys

port = 5000
socket_connection = SocketConnection(5000)
config = Configuration()
ev3 = EV3Brick()
conveyor_motor = Motor(Port.D)
rejection_motor = Motor(Port.A)
colour_sensor = ColorSensor(Port.S4)


ev3.screen.draw_text(0,0,"Connect")
ev3.speaker.say("Waiting for connection")
ev3.screen.draw_text(0,20,"@ " + str(port))


try:
    socket_connection.start()
except:
    ev3.speaker.say("Failed to initialise a socket server on the port " + str(port) + ". Exiting...")
    sys.exit()

ev3.screen.clear()
ev3.speaker.beep()

ev3.screen.draw_text(0,0,"Running")

conveyor_motor.run(-225)

previous_col = None

while True:
    if Button.CENTER in ev3.buttons.pressed():
        break

    detected_colour = colour_sensor.color()
    if detected_colour != None and ((config.isBlacklist and detected_colour in config.colour_list) or ((not config.isBlacklist) and not (detected_colour in config.colour_list))):
        previous_col = None
        socket_connection.dispatch(Payload(1,{"Colour":detected_colour, "IsRejected":"true"}))
        time.sleep(0.3)
        rejection_motor.run_angle(864,360, wait= True)
    elif (detected_colour != None):
        if(previous_col != detected_colour):
            socket_connection.dispatch(Payload(1,{"Colour":detected_colour, "IsRejected":"false"}))
        if(previous_col == None):
            previous_col = detected_colour
    else:
        previous_col = None

conveyor_motor.stop()
ev3.speaker.beep()

socket_connection.close()