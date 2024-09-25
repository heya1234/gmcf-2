// randomSpeedGen.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <ctime>
#include <random>
#include <Windows.h>

int main()
{
    std::default_random_engine rand;
    std::uniform_real_distribution<double> u(0.0, 30.0);
    rand.seed(time(0));

    float myRandomNumber = 0.0f;
    while (1)
    {
        myRandomNumber = u(rand);
        std::cout << myRandomNumber << std::endl;
        Sleep(1000);
    }
    return 0;
}
