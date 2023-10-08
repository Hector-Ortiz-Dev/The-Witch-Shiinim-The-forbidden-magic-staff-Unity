#include<Wire.h>

int data;
const int motor=10;
const int led_rojo=4;
const int led_verde=5;
const int led_amarillo=6;
const int led_blanco=7;
const int MPU=0x68;
int16_t Gx,Gy,Gz,Tmp,Ax,Ay,Az;

void setup() {
  Wire.begin();
  Wire.beginTransmission(MPU);
  Wire.write(0x6B); // PWM_MGMT_1 register
  Wire.write(0);
  Wire.endTransmission(true);
  Serial.begin(9600);
  pinMode(motor,OUTPUT);
  pinMode(led_rojo,OUTPUT);
  pinMode(led_verde,OUTPUT);
  pinMode(led_amarillo,OUTPUT);
  pinMode(led_blanco,OUTPUT);
}

void loop() {
GetMpuValue(MPU);
  float accel_ang_x=atan(Ax/sqrt(pow(Ay,2) + pow(Az,2)))*(180.0/3.14);
  float accel_ang_y=atan(Ay/sqrt(pow(Ax,2) + pow(Az,2)))*(180.0/3.14);
   Serial.print("Inclinacion en X: ");
  Serial.print(accel_ang_x); 
  Serial.print("||||Inclinacion en Y:");
  Serial.println(accel_ang_y);
  if(Serial.available()){
    data=Serial.read();
    if(data=='A'){
      digitalWrite(led_rojo,HIGH);
    }
    else{
      digitalWrite(led_rojo,LOW);
    }
       if(data=='B'){
      digitalWrite(led_verde,HIGH);
    }
    else{
      digitalWrite(led_verde,LOW);
    }
        if(data=='C'){
      digitalWrite(led_amarillo,HIGH);
    }
    else{
      digitalWrite(led_amarillo,LOW);
    }
           if(data=='D'){
      digitalWrite(led_blanco,HIGH);
    }
    else{
      digitalWrite(led_blanco,LOW);
    }
    if(data=='E'){
      digitalWrite(motor,HIGH);
    }
    else{
      digitalWrite(motor,LOW);
    }
    if(data=='F'){//Grados de giroscopio
       Serial.println(accel_ang_x);
    }
    if(data=='G'){ 
     Serial.println(accel_ang_y);
    }
   
  
    delay(50);
  }
}

void GetMpuValue(const int MPU){
  Wire.beginTransmission(MPU);
  Wire.write(0x3B);
  Wire.endTransmission(false);
  Wire.requestFrom(MPU, 14, true);
  Ax=Wire.read()<<8|Wire.read();
  Ay=Wire.read()<<8|Wire.read();
  Az=Wire.read()<<8|Wire.read();
  Tmp=Wire.read()<<8|Wire.read();
  Gx=Wire.read()<<8|Wire.read();
  Gy=Wire.read()<<8|Wire.read();
  Gz=Wire.read()<<8|Wire.read();
}
