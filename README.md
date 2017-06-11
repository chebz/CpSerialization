## CpGames Serialization

Did life get you down? Do you feel like you are missing something important to be happy? Well look no further! Custom serializers that store type information are here! Why would you ever need them? I don't know! But it's open-source, so you can add your own serializers or types and attain true happiness!

#### Dictionary Serializer:

Serializes your classes to Dictionary(string, object) where string is a field name and object is a primitive data type. I use it with Photon Server protocol.

#### Document Serializer:

Serializes your classes to AWS DynamoDB Document type and back. Now you can store your classes in aws database (without using their DataModel pipeline that totally disrepects abstract or interface). Woohoo!