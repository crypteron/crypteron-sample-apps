# Crypteron CipherObject Sample

This is a sample application showing how to use CipherObject to encrypt sensitive data bearing objects using strong encryption and robust key management.

## Flexibility

By operating at a fundamental `object` level, CipherObject gives you the ultimate flexibility you can now use data encryption and strong key management with any workflow. Want to send encrypted data into a message queue/database/REST endpoint? Just encrypt the object by `myObject.Encrypt()` before sending it as usual.

## Output

```
Object
------
{
  "Id": 0,
  "Name": "John Doe",
  "SocialSecurityNumber": "555-55-5555"
}
Encrypted
---------
{
  "Id": 0,
  "Name": "zbMAAAIAGKQhHqUPv/WxtsPh1QAAAESexLvksQqsigYBzmi+W8tZ70Rpt0tZ/w==",
  "SocialSecurityNumber": "zbMAAAIAGNy2GbcZ5ZPcef2wWQAAAJHipZfGA4ZOVbtNPOgiFqjBjb8VcsCTkrs2+g=="
}
Decrypted
---------
{
  "Id": 0,
  "Name": "John Doe",
  "SocialSecurityNumber": "555-55-5555"
}

Additional documentation at https://www.crypteron.com/docs or contact support(at)crypteron.com for assistance

Press enter to exit ...
```

## Entity Framework

If you're working with Entity Framework, you may want to check the Crypteron CipherDb sample applications as CipherDb automatically manages the encryption and decryption of object for you.

## Support

If you have any questions, please reach out to us at support(at)crypteron.com