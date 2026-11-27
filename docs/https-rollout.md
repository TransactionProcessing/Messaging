# MessagingService HTTPS rollout

MessagingService serves HTTPS only on its existing port `5006`. It does not
provide an HTTP listener. The existing SMTP2GO and The SMS Works endpoints are
unrelated and remain unchanged.

## Configure the HTTPS listener

Set these environment variables in the development or staging service
environment before starting the service:

```text
MessagingService__Https__CertificatePath=<path to mounted CA-issued PFX>
MessagingService__Https__CertificatePassword=<secret-store value>
```

Mount the CA-issued server certificate into the service environment and provide
its password through the deployment secret store. Do not add the certificate,
private key, or password to this repository or a container image. The
certificate must be valid for the hostname callers use, and callers must trust
its issuing CA.

The certificate must be mounted before startup. The service fails to start if
the certificate path is missing or the certificate cannot be loaded; it never
falls back to HTTP. The certificate password may be empty if the PFX has no
password.

The container images no longer import repository certificate files or PFX
files. Provision the issuing CA trust into the host/runtime trust store through
the deployment environment so health checks and callers can validate the
certificate chain.

## Health checks and rollout

Probe `https://localhost:5006/healthui` from a host that trusts the issuing CA.
Also verify that a client with an untrusted root and a client using a hostname
that does not match the certificate both reject the HTTPS connection.

The outbound SecurityService migration is tracked separately in Messaging issue
#458.
