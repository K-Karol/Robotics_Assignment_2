#!/usr/bin/env pybricks-micropython
from pybricks.hubs import EV3Brick
from pybricks.ev3devices import (Motor, TouchSensor, ColorSensor,
                                 InfraredSensor, UltrasonicSensor, GyroSensor)
from pybricks.parameters import Port, Stop, Direction, Button, Color
from pybricks.tools import wait, StopWatch, DataLog
from pybricks.robotics import DriveBase
from pybricks.media.ev3dev import SoundFile, ImageFile
import socket

port = 5000
ev3 = EV3Brick()
client = None


addr = socket.getaddrinfo('0.0.0.0', port)[0][-1]

s = socket.socket()
s.bind(addr)
s.listen(1)

ev3.screen.draw_text(10,10,"Waiting for operator connection...")
ev3.screen.draw_text(10,20,"Hosting on port " + str(port))
client, client_address = s.accept()
ev3.screen.clear()
ev3.speaker.beep()