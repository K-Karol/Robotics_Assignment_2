class Payload:
    '''This class is used to represent a payload received/sent over a socket connection. The class contains a `commandID` property (representing the Command ID number),
    and `paramaters` property which is a Dictionary of Keys (string) and Values (string) which are parsed as command arguments'''
    commandID = 0,
    paramaters = {}
    def __init__(self, commandID, paramaters):
        self.commandID = commandID
        self.paramaters = paramaters
    def Parse(toParse):
        """Parse a string and returns a Payload if successful, or None if not"""
        toParse = toParse.replace(';','')
        parts = toParse.split('|')
        if(len(parts) < 1):
            return None
        if(not parts[0].startswith("#")):
            return None
        
        commandID = int(parts[0].replace('#',''))

        if(commandID <= 0):
            return None
        
        params = {}

        for p in parts[1:]:
            splitP = p.split(":")
            if(len(splitP) != 2):
                return None
            params[splitP[0]] = splitP[1]
        
        return Payload(commandID, params)
    def ToString(self):
        """Encodes the Payload as a string and returns the string for transmission over the socket."""
        s = "#{}".format(self.commandID)
        if len(self.paramaters) < 0:
            s += ";"
            return s
        for p in self.paramaters:
            s += "|{k}:{v}".format(k=p,v=self.paramaters[p])
        s += ";"
        return s