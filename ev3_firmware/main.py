#!/usr/bin/env pybricks-micropython
from pybricks.hubs import EV3Brick
from pybricks.ev3devices import (Motor, TouchSensor, ColorSensor,
                                 InfraredSensor, UltrasonicSensor, GyroSensor)
from pybricks.parameters import Port, Stop, Direction, Button, Color
from pybricks.tools import wait, StopWatch, DataLog
from pybricks.robotics import DriveBase
from pybricks.media.ev3dev import SoundFile, ImageFile
from socket_connection import SocketConnection
import time
from payload import Payload

port = 5000
ev3 = EV3Brick()
socket_connection = SocketConnection(5000)

ev3.screen.draw_text(0,0,"Connect")
ev3.speaker.say("Waiting for connection")
ev3.screen.draw_text(0,20,"@ " + str(port))

socket_connection.start()

ev3.screen.clear()
ev3.speaker.beep()

ev3.screen.draw_text(0,0,"Running")

while True:
    if Button.CENTER in ev3.buttons.pressed():
        break



ev3.speaker.beep()

socket_connection.close()