class Payload:
    commandID = 0,
    paramaters = {}
    def __init__(self, commandID, paramaters):
        self.commandID = commandID
        self.paramaters = paramaters
    def Parse(toParse):
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
        s = "#{}".format(self.commandID)
        if len(self.paramaters) < 0:
            s += ";"
            return s
        for p in self.paramaters:
            s += "|{k}:{v}".format(k=p,v=self.paramaters[p])
        s += ";"
        return s