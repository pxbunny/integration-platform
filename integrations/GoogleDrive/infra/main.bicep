param projectName string
param integrationName string
param sharedAppServicePlanName string
param sharedStorageAccountName string
param sharedKeyVaultName string
param cronSchedule string
param timeZone string

module storageAccount '../../../shared-infra/modules/storageAccount.bicep' = {
  name: 'storageAccountDeploy'
  params: {
    projectName: projectName
    integrationName: integrationName
  }
}

module functionApp '../../../shared-infra/modules/functionApp.bicep' = {
  name: 'functionAppDeploy'
  params: {
    projectName: projectName
    integrationName: integrationName
    customAppSettings: [
      {
        name: 'CronSchedule'
        value: cronSchedule
      }
      {
        name: 'StorageAccountConnectionString'
        value: storageAccount.outputs.connectionString
      }
    ]
    sharedAppServicePlanName: sharedAppServicePlanName
    sharedStorageAccountName: sharedStorageAccountName
    sharedKeyVaultName: sharedKeyVaultName
    timeZone: timeZone
  }
}

output appName string = functionApp.outputs.appName
