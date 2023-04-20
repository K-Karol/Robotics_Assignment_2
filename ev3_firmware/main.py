#!/usr/bin/env pybricks-micropython

# The above line is needed to execute on the ev3dev
from pybricks.hubs import EV3Brick
from pybricks.ev3devices import (Motor, TouchSensor, ColorSensor,
                                 InfraredSensor, UltrasonicSensor, GyroSensor)
from pybricks.parameters import Port, Stop, Direction, Button, Color
from pybricks.tools import wait, StopWatch, DataLog
from pybricks.robotics import DriveBase
from pybricks.media.ev3dev import SoundFile, ImageFile

import time
import sys

from socket_connection import SocketConnection
from configuration import Configuration
from payload import Payload

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
    socket_connection.start() # waiting for a connection
except:
    ev3.speaker.say("Failed to initialise a socket server on the port " + str(port) + ". Exiting...") # This happens often as the socket for some reason isn't fully closed sometimes, so we have to wait till the OS frees the socket.
    sys.exit()

ev3.screen.clear()
ev3.speaker.beep()

ev3.screen.draw_text(0,0,"Running")

conveyor_motor.run(-225)

previous_col = None
# main loop
while True: # Main loop
    if Button.CENTER in ev3.buttons.pressed():
        break # stops the application is the middle button is pressed

    detected_colour = colour_sensor.color() # detects colour
    if detected_colour != None and ((config.isBlacklist and detected_colour in config.colour_list) or ((not config.isBlacklist) and not (detected_colour in config.colour_list))): # Checks if the colour needs to be rejected based on the configuration.
        previous_col = None
        #time_previous_col = time.ticks_ms()
        socket_connection.dispatch(Payload(1,{"Colour":detected_colour, "IsRejected":"true"})) # dispatches a message to the client with info. of the rejected item
        time.sleep(0.3)
        rejection_motor.run_angle(864,360, wait= True) # pushes the item of the conveyour belt.
    elif (detected_colour != None):
        if(previous_col != detected_colour): # this is my attempt to not scan the same vegetable as multiple items when it loops around/
            socket_connection.dispatch(Payload(1,{"Colour":detected_colour, "IsRejected":"false"})) #  dispatches a message to the client with info. of the non-rejected item
            previous_col = detected_colour
    else:
        previous_col = None

    #and time.ticks_diff(time.ticks_ms(), time_previous_col) > 300

    popped_payload = socket_connection.payloads.pop() # Checks for messages (payloads) from the client.
    if popped_payload != None: # If true, a message needs to be processed
        if popped_payload.commandID == 1: # GetCurrentConfigurationCommand
            all_cols_string = ','.join(map(str, config.colour_list))
            socket_connection.dispatch(Payload(2,{"IsBlackList": "true" if config.isBlacklist else "false", "ColourList":all_cols_string})) # Sends a payload with the current configuration properties
        elif popped_payload.commandID == 2: # UpdateConfigurationCommand
            # The following code parses the payload values and sets the configuration.
            config.isBlacklist = True if popped_payload.paramaters["IsBlackList"] == "true" else False
            config.colour_list.clear()
            for col in popped_payload.paramaters["ColourList"].split(","):
                # config.colour_list.append(Color[col.split("Color.")[1]])
                if col == "Color.BLACK":
                    config.colour_list.append(Color.BLACK)
                elif col == "Color.RED":
                    config.colour_list.append(Color.RED)
                elif col == "Color.YELLOW":
                    config.colour_list.append(Color.YELLOW)
                elif col == "Color.GREEN":
                    config.colour_list.append(Color.GREEN)
                elif col == "Color.BLUE":
                    config.colour_list.append(Color.BLUE)
                elif col == "Color.MAGENTA":
                    config.colour_list.append(Color.MAGENTA)
                elif col == "Color.BROWN":
                    config.colour_list.append(Color.BROWN)
                elif col == "Color.WHITE":
                    config.colour_list.append(Color.WHITE)
            
            all_cols_string = ','.join(map(str, config.colour_list))
            socket_connection.dispatch(Payload(2,{"IsBlackList": "true" if config.isBlacklist else "false", "ColourList":all_cols_string})) # Sends a payload with the current configuration properties to force an update in the client

conveyor_motor.stop()
ev3.speaker.beep()

socket_connection.close()