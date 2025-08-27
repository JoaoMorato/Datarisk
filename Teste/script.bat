@echo off
curl -X POST -H "Content-Type: multipart/form-data" -F "file=@script.js" http://localhost:7000/Script/teste