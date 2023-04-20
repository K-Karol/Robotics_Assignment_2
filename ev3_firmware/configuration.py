from pybricks.parameters import Color
class Configuration:
    '''This class stores the configuration details of the robot.'''
    colour_list = []
    isBlacklist = False
    def __init__(self):
        self.colour_list = [Color.RED]