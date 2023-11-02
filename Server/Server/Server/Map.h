#pragma once
constexpr int W_WIDTH = 500;
constexpr int W_HEIGHT = 500;
constexpr int TEXT_WIDTH = 200;
constexpr int TEXT_HEIGHT = 200;

constexpr char WALL = '1';

extern char hometown[W_HEIGHT][W_WIDTH];
extern char stage1[W_HEIGHT][W_WIDTH];
extern char stage2[W_HEIGHT][W_WIDTH];
extern char stage3[W_HEIGHT][W_WIDTH];

void load_map();