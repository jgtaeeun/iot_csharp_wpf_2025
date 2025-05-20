## 파이썬 Mqtt Publish


import paho.mqtt.client as mqtt
import json
import datetime as dt
import uuid
from collections import OrderedDict
import random
import time   #스레드 위한 timer

PUB_ID = 'IOT110'                     
BROKER = '210.119.12.110'          
PORT = 1883                        
TOPIC = 'smarthome/110/topic'    
COLORS = ['RED', 'ORANGE', 'YELLOW', 'GREEN', 'BLUE', 'NAVY', 'PUPPLE']
COUNT = 0                            



# [fake] 센서 설정
SENSOR1 = "온도센서셋팅"; PIN1 = 5
SENSOR2 = "포토센서셋팅"; PIN2 = 7
SENSOR3 = "워터드롭센서셋팅"; PIN3= 9
SENSOR4 = "인체감지센서셋팅"; PIN4=11
SENSOR5 = "습도센서셋팅"; PIN4=13


def on_connect(client, userdata, flags, reason_code, properties = None):
    print(f'Connectedc with reason code : {reason_code}')


def on_publish(client, userdata,mid) :
    print(f'Message published mid : {mid}')


try :
    client = mqtt.Client(client_id=PUB_ID , protocol=mqtt.MQTTv5 )
    client.on_connect = on_connect
    client.on_publish = on_publish

    client.connect(BROKER ,PORT)
    client.loop_start()

    while True :
        currTime = dt.datetime.now()
        selected = random.choice(COLORS)
        temp = random.uniform(20.0,29.9)
        humid = random.uniform(40.0,65.9)
        rain = random.randint(0,1)         #0은 맑음, 1은 비
        person = random.randint(0,1)      #0은 사람감지안됨. 1은 사람감지됨
        light = random.randint(50, 255)   #50은 낮 255는 밤

        COUNT += 1
        #센싱데이터를  json형태로 변경
        # OrderedDict로 먼저 구성. 순서가 있는 딕셔너리 타입 객체
        raw_data = OrderedDict()
        raw_data['PUB_ID'] = PUB_ID
        raw_data['COUNT'] = COUNT
        raw_data['SENSING_DT'] = currTime.strftime(f'%Y/%m/%d %H:%M:%S')    #%d, %s가 예약어이므로 f문자열로 감싸줘야 날짜형식이 된다. 
        raw_data['TEMP'] = f'{temp:0.1f}'
        raw_data['HUMID'] =f'{humid:0.1f}'
        raw_data['RAIN'] = rain
        raw_data['PERSON'] = person
        raw_data['LIGHT'] = 1 if light >=200 else 0
        #python 딕셔너리 형태로 저장되어 있음. json이랑 거의 똑같음
        #OrderedDict ->json타입으로 변경

        pub_data = json.dumps(raw_data, ensure_ascii=False, indent='\t')
       
       #payload에 json데이터를 할당
        client.publish(TOPIC, payload =  pub_data , qos = 1)
        time.sleep(1)

except Exception as ex:
    print(f"Error raised: {ex}")


except KeyboardInterrupt :
    print('MQTT 전송중단')
    client.loop_stop()
    client.disconnect()