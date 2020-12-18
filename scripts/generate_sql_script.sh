#!/bin/bash
# Parameters PROJECT_PATH, OUTPUT_DIR, SCRIPT_NAME are provided as environment variables in the azure pipeline task that executes this script
# Generate IdentityContext Migration SQL Script
dotnet ef migrations script --idempotent --context IdentityContext --output $OUTPUT_DIR/identity.sql --project $PROJECT_PATH

# Create Unified SQL Script and Remove the Individual Scripts
cat $OUTPUT_DIR/*.sql > $OUTPUT_DIR/$SCRIPT_NAME && (find $OUTPUT_DIR -type f ! -name $SCRIPT_NAME -delete)