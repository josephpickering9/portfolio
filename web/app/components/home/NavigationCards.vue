<template>
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 w-full max-w-7xl">
    <NuxtLink to="/search" class="group card bg-base-100 hover:bg-base-200/50 shadow-md hover:shadow-2xl transition-all duration-300 border border-base-200 hover:border-info/50 hover:-translate-y-1 animate-fade-in-up" style="animation-delay: 0ms">
      <div class="card-body items-center text-center p-8">
        <div class="p-4 rounded-full bg-info/10 group-hover:bg-info/20 transition-colors duration-300 mb-4">
          <div class="flex items-center justify-center transition-transform duration-300 ease-out group-hover:scale-110 group-hover:-rotate-6">
            <Icon name="mdi:robot-happy-outline" class="!w-10 !h-10 !text-info" />
          </div>
        </div>
        <p class="card-title text-2xl mb-2 font-bold">AI Search</p>
        <p class="text-base-content/70">Ask questions about my work experience, projects and skills</p>
      </div>
    </NuxtLink>
    <NuxtLink to="/projects" class="group card bg-base-100 hover:bg-base-200/50 shadow-md hover:shadow-2xl transition-all duration-300 border border-base-200 hover:border-primary/50 hover:-translate-y-1 animate-fade-in-up" style="animation-delay: 80ms">
      <div class="card-body items-center text-center p-8">
        <div class="p-4 rounded-full bg-primary/10 group-hover:bg-primary/20 transition-colors duration-300 mb-4">
          <div class="flex items-center justify-center transition-transform duration-300 ease-out group-hover:scale-110 group-hover:translate-y-[-2px]">
            <Icon name="heroicons:rectangle-stack" class="!w-10 !h-10 !text-primary" />
          </div>
        </div>
        <p class="card-title text-2xl mb-2 font-bold">Projects</p>
        <p class="text-base-content/70">View all my projects</p>
      </div>
    </NuxtLink>
    <NuxtLink to="/companies" class="group card bg-base-100 hover:bg-base-200/50 shadow-md hover:shadow-2xl transition-all duration-300 border border-base-200 hover:border-secondary/50 hover:-translate-y-1 animate-fade-in-up" style="animation-delay: 160ms">
      <div class="card-body items-center text-center p-8">
        <div class="p-4 rounded-full bg-secondary/10 group-hover:bg-secondary/20 transition-colors duration-300 mb-4">
          <div class="flex items-center justify-center transition-transform duration-300 ease-out group-hover:scale-110 group-hover:rotate-3">
            <Icon name="heroicons:building-office" class="!w-10 !h-10 !text-secondary" />
          </div>
        </div>
        <p class="card-title text-2xl mb-2 font-bold">Companies</p>
        <p class="text-base-content/70">See companies I've worked with</p>
      </div>
    </NuxtLink>
    <NuxtLink to="/technologies" class="group card bg-base-100 hover:bg-base-200/50 shadow-md hover:shadow-2xl transition-all duration-300 border border-base-200 hover:border-accent/50 hover:-translate-y-1 animate-fade-in-up" style="animation-delay: 240ms">
      <div class="card-body items-center text-center p-8">
        <div class="p-4 rounded-full bg-accent/10 group-hover:bg-accent/20 transition-colors duration-300 mb-4">
          <div class="flex items-center justify-center transition-transform duration-300 ease-out group-hover:scale-110 group-hover:-rotate-3">
            <Icon name="heroicons:code-bracket" class="!w-10 !h-10 !text-accent" />
          </div>
        </div>
        <p class="card-title text-2xl mb-2 font-bold">Technologies</p>
        <p class="text-base-content/70">Browse by technologies</p>
      </div>
    </NuxtLink>
  </div>
</template>
