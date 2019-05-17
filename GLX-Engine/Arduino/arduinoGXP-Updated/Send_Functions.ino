void SendAnalogData(int pin)
{
  if (initialiseRun)
  {
    analogs++;
  }
  String toSend = "ANALOG";
  toSend.concat(analogID++);
  toSend.concat("=");

  float analogValue = (analogRead(pin) / 511.5) - 1;
  analogValue = (float)((int)(analogValue * 10)) / 10.f;

  toSend.concat(analogValue);
  Serial.println(toSend);

}

void SendDigitalData(int pin)
{
  if (initialiseRun)
  {
    digitals++;
  }
  String toSend = "DIGITAL";
  toSend.concat(digitalID++);
  toSend.concat("=");

  bool digitalValue = digitalRead(pin);

  toSend.concat(!digitalValue);
  Serial.println(toSend);
}
