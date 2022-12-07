#!/bin/bash

aws dynamodb create-table \
    --table-name user-portfolio \
    --attribute-definitions \
        AttributeName=UserId,AttributeType=S \
    --key-schema \
        AttributeName=UserId,KeyType=HASH \
    --provisioned-throughput \
        ReadCapacityUnits=5,WriteCapacityUnits=5 \
    --table-class STANDARD
  
aws dynamodb put-item \
    --table-name user-portfolio \
    --item \
      '{"UserId": {"S": "{ YOUR PHONE NUMBER WITH COUNTRY CODE }"}, "AvailableCash": {"N": "1000"}, "StockPortfolio": {"L": []}}'

aws dynamodb create-table \
     --table-name stock-prices \
     --attribute-definitions \
         AttributeName=Name,AttributeType=S \
     --key-schema \
         AttributeName=Name,KeyType=HASH \
     --provisioned-throughput \
         ReadCapacityUnits=5,WriteCapacityUnits=5 \
     --table-class STANDARD
     
aws dynamodb put-item --table-name stock-prices --item '{"Name": {"S": "Apple"}, "Price": {"N": "144.00"} }'
aws dynamodb put-item --table-name stock-prices --item '{"Name": {"S": "Alphabet"}, "Price": {"N": "96.00"} }'
aws dynamodb put-item --table-name stock-prices --item '{"Name": {"S": "Microsoft"}, "Price": {"N": "144.00"} }'
aws dynamodb put-item --table-name stock-prices --item '{"Name": {"S": "Tesla"}, "Price": {"N": "182.00"} }'
aws dynamodb put-item --table-name stock-prices --item '{"Name": {"S": "Twilio"}, "Price": {"N": "46.00"} }'

aws iam create-role --role-name stockbrokerbot-lambda-role --assume-role-policy-document file://LambdaBasicRole.json
aws iam attach-role-policy --role-name stockbrokerbot-lambda-role --policy-arn arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole
aws iam put-role-policy --role-name stockbrokerbot-lambda-role --policy-name dynamodb-table-access --policy-document file://LambdaDynamoDBAccessPolicy.json



