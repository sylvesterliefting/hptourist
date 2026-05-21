using HPTourist.Components;
using HPTourist.Data.Models;
using HPTourist.Database;
using HPTourist.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var app = Application.Setup(args);
app.Run();
