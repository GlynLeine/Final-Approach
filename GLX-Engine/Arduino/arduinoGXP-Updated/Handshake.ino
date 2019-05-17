bool CheckHandshake(String& message)
{
  if (message.indexOf("GIVE HANDSHAKE") >= 0)
  {
    Serial.println("HANDSHAKE");
    
    Serial.print("ID");
    Serial.println(ID);
    
    Serial.print("ANALOGS");
    Serial.println(analogs);
    
    Serial.print("DIGITALS");
    Serial.println(digitals);
    
    connectedToEngine = true;

    String maxangle = "180;";
    String minangle = "0;";
    String stopAngle = "93;";

    ActuateServo(1, minangle);
    ActuateServo(1, maxangle);

    ActuateServo(2, maxangle);
    ActuateServo(2, stopAngle);
    
    return true;
  }
  else
  {
    if (message.indexOf("SEND") >= 0)
    {
      return true;
    }
  }
  return false;
}
