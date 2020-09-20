# Refactoring and Design Patterns in the real world

## Software Craftsmanship Lisbon

### 💥 This is an anemic model with several code smells 💩💩💩

### 👉 To see the transformation after the refactorings, checkout the `develop` branch 🧼

------

### **TO-DO**

- ~~Prepare presentation~~
- ~~**Tests**~~
- **Refactoring model**
  - Encapsulation:
    - Constructor added / visibility changed to private
    - Members initialized
    - Setters removed / Public interface added
    - First contracts added

- **Refactoring AppService**
  - Exception -> Notifications
  - Move Notifications AppService -> Model through contracts

- **Refactoring tests with contracts**
  - OrderTest...
  - Add HappyPath
- Replace primitive types with value objects
- Add strategy for calcs
  - Remove new Oder(command.Discount) dependency
- Analysers