﻿# معماری میکروسرویس

معماری میکروسرویس یک رویکرد در طراحی نرم‌افزار است که در آن یک برنامه بزرگ به چندین سرویس کوچک و مستقل تقسیم می‌شود	

## ساختار پروژه

OnlineStore/
├── src/
│   ├── Services/
│   │   ├── Catalog.WebApi/
│   │   ├── Basket.WebApi/
│   │   ├── Ordering.WebApi/
│   │   └── Identity.WebApi/
│   ├── ApiGateway.WebApi/
│   └── WebApp/
├── tests/
└── docker-compose.yml

### ملزومات  
	-------------------------------------------------------------------------------------------
	# ارتباط بین سرویس‌ها
		1- RabbitMQ: برای ارتباط ناهمگام بین سرویس‌ها استفاده می‌شود
		2- gRPC: برای ارتباطات سریع بین سرویسی به کار می‌رود
	-------------------------------------------------------------------------------------------
	# پیاده‌سازی و استقرار
		1- Dockerfile: باید هر سرویس داکر فایل مجزا داشته باشد
		2- docker-compose.yml: از این فایل در ریشه پروژه برای اجرای تمام سرویس‌ها استفاده می‌شود
	-------------------------------------------------------------------------------------------
### تکنولوژی ها و کتابخانه ها

	1- Ocelot : یک پروکسی برای ارتباط بین کلاینت و سرویس ها می باشد

