# Git Workflow: Ubuntu Main Machine ↔ Windows VM (Visual Studio)

This guide explains how to work with Git when developing on Ubuntu but running/testing in Visual Studio on Windows VM.

## 🎯 Quick Workflow

### On Ubuntu (Main Machine) - Development:
```bash
# 1. Make your changes
# ... edit files ...

# 2. Stage your changes
git add .

# 3. Commit with descriptive message
git commit -m "Fix: Added anti-forgery tokens to all POST forms"

# 4. Push to GitHub
git push origin main
```

### On Windows VM (Visual Studio) - Testing:
```bash
# 1. Pull latest changes
git pull origin main

# 2. Open in Visual Studio and test/run
# ... Visual Studio will restore packages automatically ...

# 3. If you make changes (fixes, testing notes, etc.)
git add .
git commit -m "Fix: Windows-specific adjustments"
git push origin main
```

### On Ubuntu - Pull Windows VM changes:
```bash
git pull origin main
```

---

## 🔧 Initial Setup (One-time on each machine)

### On Ubuntu:
```bash
cd /home/rajae/Downloads/CoworkingReservation

# Configure Git for cross-platform (already done)
git config core.autocrlf input
git config core.eol lf

# Verify your Git config
git config --local --list
```

### On Windows VM (Visual Studio):
```bash
cd C:\Path\To\CoworkingReservation  # or wherever you clone it

# Clone the repository if not already cloned
git clone https://github.com/mokhliss12/coworking-reservation.git

# Configure Git for Windows
git config core.autocrlf true
git config core.eol crlf

# OR if you want to keep LF line endings (recommended for .NET):
git config core.autocrlf input
git config core.eol lf
```

---

## 📋 Recommended Daily Workflow

### Option 1: Branch-Based (Recommended)
```bash
# On Ubuntu - Create feature branch
git checkout -b feature/fix-login-dependency
# ... make changes ...
git add .
git commit -m "Fix login dependency issues"
git push origin feature/fix-login-dependency

# On Windows VM - Test the branch
git fetch origin
git checkout feature/fix-login-dependency
# Test in Visual Studio...

# If fixes needed on Windows VM:
git commit -am "Fix: Windows-specific adjustments"
git push origin feature/fix-login-dependency

# Back on Ubuntu - Merge when ready
git checkout main
git merge feature/fix-login-dependency
git push origin main
```

### Option 2: Direct Main Branch (Simpler)
```bash
# Always pull before starting work
git pull origin main

# Make changes, commit, push
git add .
git commit -m "Description of changes"
git push origin main

# On Windows VM - pull and test
git pull origin main
```

---

## ⚠️ Important Notes

### 1. **Database Connection Strings**
The `appsettings.json` might have different connection strings:
- **Ubuntu**: `Server=(localdb)\\MSSQLLocalDB;...`
- **Windows VM**: Might need different connection string

**Solution**: Keep a separate `appsettings.Development.json` that's in `.gitignore`:
```bash
# Add to .gitignore
appsettings.Development.json
appsettings.Production.json
```

Or use environment-specific connection strings.

### 2. **Line Endings**
- Git is configured to handle line endings automatically
- On Ubuntu: Uses LF
- On Windows: Will convert to CRLF when needed (if configured)

### 3. **Build Artifacts**
- `bin/`, `obj/` folders are already in `.gitignore`
- Visual Studio will restore packages automatically
- Run `dotnet restore` if needed after pulling

### 4. **NuGet Packages**
- Packages are restored automatically by Visual Studio
- If issues occur: `dotnet restore`

### 5. **EF Core Migrations**
- Migrations are tracked in Git (good!)
- Run migrations on both machines if needed:
```bash
dotnet ef database update --project Coworking.Infrastructure --startup-project ../Coworking.Web
```

---

## 🔍 Common Commands

### Check Status
```bash
git status
```

### See What Changed
```bash
git diff
```

### Undo Uncommitted Changes
```bash
git restore <file>
# or
git checkout -- <file>
```

### Create a Backup Branch Before Major Changes
```bash
git checkout -b backup/before-major-change
git checkout main
```

### See Commit History
```bash
git log --oneline
```

---

## 🚀 Visual Studio Integration

### In Visual Studio on Windows VM:
1. **File → Open → Project/Solution** → Open `CoworkingReservation.sln`
2. Visual Studio will automatically detect Git
3. Use **View → Team Explorer** or **View → Git Changes** for Git operations
4. Visual Studio has built-in Git support - no need for command line!

### Visual Studio Git Features:
- **Git Changes**: Shows modified files
- **Commit**: Commit directly from VS
- **Sync**: Pull and push in one click
- **Branches**: Switch branches easily
- **History**: View commit history

---

## 📦 Sharing Files Between Ubuntu and Windows VM

### Option 1: GitHub (Recommended - What you're using)
- Works across any machines
- Version control
- Backup included

### Option 2: Shared Folder (VM)
- If using VMware/VirtualBox shared folders
- Can work but can cause Git line ending issues
- Not recommended for Git repos

### Option 3: Network Share
- Similar to shared folders
- Can have permission/line ending issues

**Best Practice**: Always use GitHub for syncing code!

---

## ✅ Best Practices

1. **Always pull before starting work**: `git pull origin main`
2. **Commit frequently**: Small, logical commits
3. **Write clear commit messages**: "Fix: ...", "Add: ...", "Update: ..."
4. **Push regularly**: Don't keep changes only local
5. **Create branches** for major features: `git checkout -b feature/name`
6. **Test before pushing**: Make sure code compiles
7. **Keep `.gitignore` updated**: Don't commit build artifacts

---

## 🐛 Troubleshooting

### "Your branch is behind 'origin/main'"
```bash
git pull origin main
```

### "Merge conflicts"
```bash
# See conflicts
git status

# Resolve conflicts in files, then:
git add .
git commit -m "Resolve merge conflicts"
```

### "Remote changes not visible"
```bash
git fetch origin
git pull origin main
```

### "Line ending issues"
```bash
# On Ubuntu
git config core.autocrlf input

# On Windows
git config core.autocrlf true
# Then re-clone or:
git rm --cached -r .
git reset --hard
```

---

## 📝 Current Repository Status

**Remote**: `https://github.com/mokhliss12/coworking-reservation`
**Branch**: `main`
**Git Config**: Configured for cross-platform (LF line endings)

**Recent Changes** (not yet committed):
- Fixed login dependency issues
- Added anti-forgery tokens to all POST forms
- Security improvements in Reservations controller
- UI improvements in Home/Index

**Next Steps**:
1. Review your changes: `git status`
2. Stage changes: `git add .`
3. Commit: `git commit -m "Fix: Login dependency and security improvements"`
4. Push: `git push origin main`
5. Pull on Windows VM: `git pull origin main`

