#!/usr/bin/env pybricks-micropython
from pybricks.hubs import EV3Brick
from pybricks.ev3devices import (Motor, TouchSensor, ColorSensor,
                                 InfraredSensor, UltrasonicSensor, GyroSensor)
from pybricks.parameters import Port, Stop, Direction, Button, Color
from pybricks.tools import wait, StopWatch, DataLog
from pybricks.robotics import DriveBase
from pybricks.media.ev3dev import SoundFile, ImageFile

ev3 = EV3Brick()

conveyor_motor = Motor(Port.D)
rejection_motor = Motor(Port.A)
colour_sensor = ColorSensor(Port.S4)

ev3.speaker.beep()

conveyor_motor.run(-180)


while True:
    detected_colour = colour_sensor.color()
    if detected_colour != None:
        ev3.speaker.beep()
        rejection_motor.run_angle(720,360)
