# Yamltron
Makes conversion between clear text and Base64 of Kubernets secrets stored in a YAML file a breeze.

## How to use

### Command-line interface

`ytron -s|-c <INPUT FILE> [<OUTPUT FILE>]`

* -s : Convert secrets from clear text to Base64 for secrets
* -c : Convert secrets from Base64 to clear text

If no output file is specified, the file is converted in-place.

*Compiles to `ytron.exe` for brevity.

### Convert data nodes from clear text to base 64
`ytron -s secrets.yaml`

Alternately

`ytron -s secrets_cleartext.yaml secrets_b64.yaml`

### Convert data nodes from base 64 to clear text
`ytron -c secrets.yaml`

Alternately

`ytron -s secrets_b64.yaml secrets_cleartext.yaml`

## Example

Given the following secrets file in clear text that you want to `kubectl apply`:

```
apiVersion: v1
kind: Secret
metadata:
  name: mysecret
type: Opaque
data:
  username: Rocky Balboa
  password: southp4w
...
```

After running Yamltron (`ytron`) on this you will get:

```
apiVersion: v1
kind: Secret
metadata:
  name: mysecret
type: Opaque
data:
  username: Um9ja3kgQmFsYm9h
  password: c291dGhwNHc=
...
```
