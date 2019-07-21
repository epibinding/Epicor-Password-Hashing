# Epicor Password Hashing Algorithm

From a high-level Epicor has a little helper class to hash the password via 2 helper methods **ComputeHash**, **VerifyHash** it does support different algorithm's such as _SHA1, SHA256, SHA384, SHA512, MD5_ but by default Epicor uses **SHA256**.

That helper lies in Epicor.System.dll usually stored in the bin folder on the IIS App Server.

High Level **ComputeHash**:
1. Using RNGCryptoServiceProvider creates a **random number salt** as bytes
2. Converts your plain-text password string into bytes
3. Creates a buffer with the size of passwordBytes.Length + saltBytes.Length
4. Combines the passwordBytes and saltBytes into a single buffer array
5. Hash the buffer using **SHA256** Managed
6. Do some magic via a for loop
7. Encode **Base64**

What you need to know is that the Random Salt is embedded in the end-result Hash and is used by the VerifyHash method.

If you were to call ComputeHash a thousand times with the same password string you would get a different Hash always.

| Password | Computed Epicor Hash w/ RNG Salt |
| ------------- | ------------- |
| manager | 46uIY6/nQjHL5mX1KeE/7NtEXD3MIOblGxpVRH5ZWXNORGsNwT3WHg== |
| manager | JIzKviG6ZG5rE52KOeKEg4AvPnU72FOUQ0y7y/R8h8GZNQaV2G+GHw== | 
| manager | tjk/TfHesmjnhq7NcUbCkfQKJnqfV7Q2mJRG6hwYRD8xGIofA6tGIQ== | 

Example:
```csharp
// Returns: 46uIY6/nQjHL5mX1KeE/7NtEXD3MIOblGxpVRH5ZWXNORGsNwT3WHg== for example.
Epicor.Security.Cryptography.SHA.Hasher.ComputeHash("manager");
```
---

High Level **VerifyHash** (returns boolean if hash matched):
1. Decode Base64
2. Extract salt bytes from Hash
3. Using the same salt bytes and user provided plain-text password we re-create the Hash, only this time because we pass in a salt array ComputerHash won't create a RNG Salt it will use the one passed.
4. Compares the newly created Hash (base64) to Hash in Database (previously generated). They should Match

Example:
```csharp
Epicor.Security.Cryptography.SHA.Hasher.VerifyHash("manager", "46uIY6/nQjHL5mX1KeE/7NtEXD3MIOblGxpVRH5ZWXNORGsNwT3WHg==");
```
---

You can write alot more details on this process. But things to know that a Hash Computed on My PC would work just as fine on your machine, it does not make use of the DPAPI, hence when you copy your Database to another server, it all works just fine.

For those curious what how the Salt is created, since it's not a hard-coded "MfgSys" salt in E10.1+
```csharp
// Fills an array of bytes with a cryptographically strong sequence of random nonzero values
byte[8] saltBytes = new byte[8];
new RNGCryptoServiceProvider().GetNonZeroBytes(saltBytes)
```
You can with some effort, look up the Hashing Algorithm and re-create it easily in Visual Studio for your custom needs.

For Reference.

------

You could modify the code and remove the RNG Salt and pass in your own salt. Would work as well, only then your Hash would always be the same. Perhaps if you are re-creating this in a VBScript for some Database Copy Script, its easier to re-create.
