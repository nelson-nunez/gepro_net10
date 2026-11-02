# Angular — Notas de Frontend Alternativo

Notas sobre un posible frontend alternativo en Angular. Tutorial de referencia, aún no implementado.

## Tutorial de referencia

- [Video tutorial en YouTube](https://www.youtube.com/watch?v=w5xhZZx031I&list=PL42UNLc8e48RINrNGumxAKulG5CWgs_yv)

## Instalación

### 1. Visual Studio Installer

Instalar los siguientes componentes:

- ASP.NET Development
- Node.js development

### 2. Node.js

Instalar en la PC desde [nodejs.org/es](https://nodejs.org/es), o mediante `fnm` (Fast Node Manager):

```powershell
winget install Schniz.fnm
fnm install 22

# Verificar versiones
node -v   # Deberia imprimir "v22.14.0"
npm -v    # Deberia imprimir "10.9.2"
```

#### Error de permisos de PowerShell

Si aparece un error de permisos al ejecutar scripts de npm/fnm:

```powershell
Get-ExecutionPolicy -List
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### 3. Angular CLI

```powershell
npm install -g @angular/cli
```