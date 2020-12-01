import argparse
import concurrent.futures
from datetime import datetime
import logging
import requests
import sys

BASE_URL = "http://localhost:5000/api/"

def Create(id, secret):
    try:
        request = {
            "ClientId": id,
            "Secret": secret
        }
        response = requests.post("{0}Token/ClientCredential".format(BASE_URL), json=request, verify=False)        
        logging.debug("Create token status {}".format(response.status_code))
        return response.text
    except:
        logging.error(sys.exc_info()[0])            
        raise

def GetArgs():
    parser = argparse.ArgumentParser()
    parser.add_argument("--id", required=True)
    parser.add_argument("--secret", required=True)
    parser.add_argument("--log", default="info")
    return parser.parse_args()

if __name__ == "__main__":    
    args = GetArgs()
    logLevel = getattr(logging, args.log.upper())
    logging.basicConfig(level=logLevel)
    logging.info(Create(args.id, args.secret))
    logging.info("Complete")