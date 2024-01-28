const http = require('http')
const fs = require('fs')

const services = ['http://localhost:5105/swagger/v1/swagger.json']

for (let i = 0; i < services.length; i++) {
  const service = services[i]

  const file = fs.createWriteStream('./api/docs/swagger.json')
  http.get(service, (response) => {
    response.pipe(file)

    file.on('finish', () => {
      file.close()
      // eslint-disable-next-line no-console
      console.log(`Download of ${service} complete.`)
    })
  })
}
