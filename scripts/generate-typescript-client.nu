#!/usr/bin/env nu
# Build the API, export the OpenAPI description, and generate a TypeScript client.

let project = "UserManagement.csproj"
let configuration = ($env.CONFIGURATION? | default "Debug")
let target_framework = "net9.0"
let artifacts_dir = "artifacts"
let swagger_path = $"${artifacts_dir}/swagger.json"
let client_dir = "clients/typescript"

if not (path exists $artifacts_dir) {
    mkdir $artifacts_dir
}

if (path exists $client_dir) {
    rm -r $client_dir
}

mkdir $client_dir

print "Restoring dotnet tools..."
dotnet tool restore

print "Building project..."
dotnet build $project --configuration $configuration

let assembly_path = $"bin/${configuration}/${target_framework}/UserManagement.dll"

if not (path exists $assembly_path) {
    error make {msg: $"Assembly not found at ${assembly_path}. Ensure the project builds successfully."}
}

print "Exporting OpenAPI document..."
dotnet swagger tofile --output $swagger_path $assembly_path v1

print "Generating TypeScript client..."
npx @openapitools/openapi-generator-cli generate \
    --generator-name typescript-fetch \
    --input-spec $swagger_path \
    --output $client_dir \
    '--additional-properties=supportsES6=true,useSingleRequestParameter=true,typescriptThreePlus=true'

print $"TypeScript client generated in ${client_dir}."
