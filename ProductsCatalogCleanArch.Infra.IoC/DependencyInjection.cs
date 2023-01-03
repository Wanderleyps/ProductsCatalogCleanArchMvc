﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductsCatalogCleanArch.Application.Interfaces;
using ProductsCatalogCleanArch.Application.Mappings;
using ProductsCatalogCleanArch.Application.Services;
using ProductsCatalogCleanArch.Domain.Interfaces;
using ProductsCatalogCleanArch.Infra.Data.Context;
using ProductsCatalogCleanArch.Infra.Data.Repositories;
using System;

namespace ProductsCatalogCleanArch.Infra.IoC
{
    /*
        * 
        * Classe responsável por tratar das injeções de dependências do projeto.
        * Faz o registro das entidades mapeando interface / classe concreta.
        * Cross cuting, expõe os serviços que podem ser utilizado em qualquer parte do projeto
        * 
        * **/
    public static class DependencyInjection
    {
        /*
         * 
         *Trata-se método de extenção, adiciona funcionalidades a um tipo já existente
         * que neste caso é a interface IServiceCollection. O mesmo deve ser static
         * assim como a classe.
         * 
         * **/
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
             //define que os arquivos de migração irão ficar na pasta onde está definido o arquivo de contexto (Infra.Data)
             b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            
            //AddScoped é recomendação para aplicações web
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            //necessário informar o nome do arquivo onde foram definidas as configurações do AutoMapper
            services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

            //registrando a DI dos handlers
            var myHandlers = AppDomain.CurrentDomain.Load("ProductsCatalogCleanArch.Application");
            services.AddMediatR(myHandlers);

            return services;
        }
    }
}
