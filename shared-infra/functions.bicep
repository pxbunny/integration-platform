var hashLength = 4

func getResourceNameWithHash(name string, hashLength int) string =>
  '${name}-${take(uniqueString(name), hashLength)}'

@export()
func getResourceName(resourceType string, name string) string =>
  '${resourceType}-${name}'

@export()
func getStorageAccountName(name string, accountNumber int) string =>
  replace(getResourceNameWithHash('st${name}${padLeft(accountNumber, 2, '0')}', hashLength), '-', '')

@export()
func getUniqueResourceName(resourceType string, name string) string =>
  getResourceNameWithHash(getResourceName(resourceType, name), hashLength)

