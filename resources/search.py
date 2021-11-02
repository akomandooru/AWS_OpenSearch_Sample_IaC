# Lambda execution starts here
import boto3
import json
import requests
from requests_aws4auth import AWS4Auth
import os 

region = os.environ.get("REGION") # For example, us-west-1
service = 'es'
credentials = boto3.Session().get_credentials()
awsauth = AWS4Auth(credentials.access_key, credentials.secret_key, region, service, session_token=credentials.token)

host = os.environ.get("ENDPOINT") # The OpenSearch domain endpoint with https://
index = 'movies'
geturl = 'https://' + host + '/' + index + '/_search'
posturl = 'https://' + host + '/' + index + '/_bulk'

def main(event, context):
    method = event['httpMethod']
    if method == "GET":
        # Put the user query into the query DSL for more accurate search results.
        # Note that certain fields are boosted (^).
        query = {
            "size": 25,
            "query": {
                "multi_match": {
                    "query": event['queryStringParameters']['q'],
                    "fields": ["title^4", "plot^2", "actors", "directors"]
                }
            }
        }

        # Elasticsearch 6.x requires an explicit Content-Type header
        headers = { "Content-Type": "application/json" }

        # Make the signed HTTP request
        r = requests.get(geturl, auth=awsauth, headers=headers, data=json.dumps(query))

        # Create the response and add some extra content to support CORS
        response = {
            "statusCode": 200,
            "headers": {
                "Access-Control-Allow-Origin": '*'
            },
            "isBase64Encoded": False
        }
        # Add the search results to the response
        response['body'] = r.text
    elif method == "POST":
        # Elasticsearch 6.x requires an explicit Content-Type header
        headers = { "Content-Type": "application/json" }

        # Make the signed HTTP request
        r = requests.put(posturl, auth=awsauth, headers=headers, data=event['body'])

        # Create the response and add some extra content to support CORS
        response = {
            "statusCode": 200,
            "headers": {
                "Access-Control-Allow-Origin": '*'
            },
            "isBase64Encoded": False
        }
        # Add the search results to the response
        response['body'] = r.text
    else:
        response = {
            "statusCode": 400,
            "headers": {},
            "body": "We accept GET and a PUT only not " + method
        }

    return response