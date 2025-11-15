param projectName string
param location string = resourceGroup().location

var storageSuffixLength = 3
var uniqueStorageSuffix = take(uniqueString(subscription().id, location), storageSuffixLength)
var projectNameWithoutHyphens = replace(projectName, '-', '')
var storageAccountName = 'st${projectNameWithoutHyphens}${uniqueStorageSuffix}'

resource storageAccount 'Microsoft.Storage/storageAccounts@2025-01-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2025-05-01' = {
  name: 'kv-${projectName}'
  location: location
  properties: {
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enableRbacAuthorization: false
    tenantId: subscription().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
}
