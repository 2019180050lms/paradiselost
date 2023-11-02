#define _CRT_SECURE_NO_WARNINGS
#include "Map.h"
#include <iostream>
#include <fstream>
using namespace std;

char hometown[W_HEIGHT][W_WIDTH];
char stage1[W_HEIGHT][W_WIDTH];
char stage2[W_HEIGHT][W_WIDTH];
char stage3[W_HEIGHT][W_WIDTH];

void create_map()
{
	// 시작방
	ofstream file_h("hometown.txt");
	cout << "Map Creating..." << endl;
	for (int z = 0; z < TEXT_HEIGHT; ++z) {
		for (int x = 0; x < TEXT_WIDTH; ++x) {
			if ((x == 6 && z == 49) || (x == 6 && z == 28) || (x == 24 && z == 28) ||
				(x == 24 && z == 8) || (x == 42 && z == 7) || (x == 42 && z == 27) ||
				(x == 49 && z == 27) || (x == 49 && z == 49) || (x == 29 && z == 49)) {
				file_h << 1;
			}
			else
				file_h << 0;
		}
		file_h << endl;
	}
	file_h.close();

	// 스테이지 1
	ofstream file_s1("stage1.txt");
	for (int z = 0; z < TEXT_HEIGHT; ++z) {
		for (int x = 0; x < TEXT_WIDTH; ++x) {
			if ((x == 184 && z == 119) || (x == 184 && z == 77) || (x == 146 && z == 77) ||
				(x == 146 && z == 91) || (x == 146 && z == 104) || (x == 146 && z == 119) ||
				(x == 101 && z == 104) || (x == 101 && z == 91) || (x == 67 && z == 91) ||
				(x == 67 && z == 104) || (x == 80 && z == 91) || (x == 80 && z == 81) ||
				(x == 89 && z == 91) || (x == 89 && z == 81) || (x == 89 && z == 19) ||
				(x == 67 && z == 12) || (x == 67 && z == 57) || (x == 80 && z == 67) ||
				(x == 53 && z == 67) || (x == 53 && z == 57) || (x == 53 && z == 46) ||
				(x == 7 && z == 46) || (x == 7 && z == 56) || (x == 0 && z == 56) ||
				(x == 0 && z == 69) || (x == 43 && z == 67) || (x == 43 && z == 80) ||
				(x == 7 && z == 80) || (x == 7 && z == 91) || (x == 0 && z == 91) ||
				(x == 0 && z == 104)) {
				file_s1 << 1;
			}
			else
				file_s1 << 0;
		}
		file_s1 << endl;
	}
	file_s1.close();

	// 스테이지 2
	ofstream file_s2("stage2.txt");
	for (int z = 0; z < TEXT_HEIGHT; ++z) {
		for (int x = 0; x < TEXT_WIDTH; ++x) {
			if ((x == 5 && z == 5) || (x == 66 && z == 5) || (x == 66 && z == 91) ||
				(x == 5 && z == 91)) {
				file_s2 << 1;
			}
			else
				file_s2 << 0;
		}
		file_s2 << endl;
	}
	file_s2.close();

	// 스테이지 3
	ofstream file_s3("stage3.txt");
	for (int z = 0; z < TEXT_HEIGHT; ++z) {
		for (int x = 0; x < TEXT_WIDTH; ++x) {
			if ((x == 162 && z == 117) || (x == 162 && z == 83) || (x == 122 && z == 117) ||
				(x == 122 && z == 104) || (x == 122 && z == 96) || (x == 122 && z == 83) ||
				(x == 113 && z == 104) || (x == 113 && z == 96) || (x == 99 && z == 104) ||
				(x == 99 && z == 96) || (x == 92 && z == 96) || (x == 92 && z == 85) ||
				(x == 92 && z == 104) || (x == 92 && z == 115) || (x == 58 && z == 115) ||
				(x == 58 && z == 104) || (x == 58 && z == 96) || (x == 58 && z == 85) ||
				(x == 50 && z == 104) || (x == 50 && z == 96) || (x == 24 && z == 96) ||
				(x == 13 && z == 104) || (x == 13 && z == 22) || (x == 24 && z == 30) ||
				(x == 75 && z == 30) || (x == 75 && z == 22)) {
				file_s3 << 1;
			}
			else
				file_s3 << 0;
		}
		file_s3 << endl;
	}
	/* 맵 이전
	for (int z = 0; z < W_HEIGHT; ++z) {
		for (int x = 0; x < W_WIDTH; ++x) {
			if ((x == 472 && z == 565) || (x == 505 && z == 591) || (x == 505 && z == 574) ||
				(x == 523 && z == 573) || (x == 523 && z == 592) || (x == 165 && z == 207) ||
				(x == 165 && z == 227) || (x == 146 && z == 227) || (x == 146 && z == 247) ||
				(x == 237 && z == 247) || (x == 255 && z == 247) || (x == 263 && z == 247) ||
				(x == 263 && z == 241)) {
				file_w << 1;
			}
			if (x == 263 && z == 233 || x == 263 && z == 228 || x == 244 && z == 228 ||
				x == 244 && z == 208 || x == 239 && z == 208 || x == 231 && z == 208 ||
				x == 266 && z == 241 || x == 266 && z == 246 || x == 266 && z == 228 ||
				x == 266 && z == 233 || x == 284 && z == 228 || x == 284 && z == 209 ||
				x == 291 && z == 209 || x == 298 && z == 209 || x == 304 && z == 209 ||
				x == 304 && z == 228 || x == 323 && z == 228 || x == 323 && z == 232 ||
				x == 323 && z == 241 || x == 323 && z == 248 || x == 326 && z == 241 ||
				x == 326 && z == 264 || x == 383 && z == 264 || x == 383 && z == 248) {
				file_w << 1;
			}
			if (x == 383 && z == 225 || x == 383 && z == 209 || x == 327 && z == 209 ||
				x == 327 && z == 232 || x == 244 && z == 205 || x == 239 && z == 205 ||
				x == 232 && z == 205 || x == 226 && z == 205 || x == 226 && z == 187 ||
				x == 244 && z == 187 || x == 284 && z == 187 || x == 284 && z == 206 ||
				x == 291 && z == 206 || x == 298 && z == 206 || x == 304 && z == 206 ||
				x == 304 && z == 186 || x == 304 && z == 108 || x == 264 && z == 108 ||
				x == 264 && z == 168 || x == 245 && z == 168 || x == 245 && z == 149 ||
				x == 240 && z == 149 || x == 228 && z == 149 || x == 165 && z == 148) {
				file_w << 1;
			}
			if (x == 165 && z == 165 || x == 143 && z == 165 || x == 143 && z == 190 ||
				x == 227 && z == 190 || x == 451 && z == 225 || x == 451 && z == 248 ||
				x == 451 && z == 200 || x == 522 && z == 200 || x == 522 && z == 275 ||
				x == 453 && z == 275 || x == 228 && z == 145 || x == 244 && z == 145 ||
				x == 232 && z == 80 || x == 240 && z == 80 || x == 208 && z == 80 ||
				x == 208 && z == 49 || x == 235 && z == 47 || x == 235 && z == 20 ||
				x == 258 && z == 20 || x == 258 && z == 47 || x == 270 && z == 47 ||
				x == 270 && z == 80 || x == 237 && z == 316 || x == 255 && z == 316) {
				file_w << 1;
			}
			if (x == 203 && z == 316 || x == 203 && z == 438 || x == 253 && z == 438 ||
				x == 253 && z == 416 || x == 253 && z == 409 || x == 253 && z == 385 ||
				x == 278 && z == 385 || x == 285 && z == 385 || x == 290 && z == 385 ||
				x == 290 && z == 314 || x == 254 && z == 438 || x == 285 && z == 387 ||
				x == 290 && z == 387 || x == 278 && z == 387 || x == 256 && z == 387 ||
				x == 256 && z == 409 || x == 256 && z == 416 || x == 256 && z == 421 ||
				x == 275 && z == 421 || x == 275 && z == 428 || x == 257 && z == 428 ||
				x == 257 && z == 438 || x == 290 && z == 438 || x == 290 && z == 419) {
				file_w << 1;
			}
			if (x == 209 && z == 406 || x == 377 && z == 468 || x == 394 && z == 406 ||
				x == 390 && z == 468 || x == 374 && z == 419 || x == 326 && z == 466 ||
				x == 317 && z == 582 || x == 326 && z == 592 || x == 317 && z == 582 ||
				x == 374 && z == 592 || x == 373 && z == 576 || x == 335 && z == 576 ||
				x == 335 && z == 535 || x == 335 && z == 521 || x == 335 && z == 482 ||
				x == 409 && z == 482 || x == 409 && z == 467 || x == 317 && z == 473 ||
				x == 383 && z == 576 || x == 383 && z == 589 || x == 383 && z == 601 ||
				x == 388 && z == 611 || x == 426 && z == 611 || x == 435 && z == 601) {
				file_w << 1;
			}
			if (x == 435 && z == 589 || x == 435 && z == 576 || x == 435 && z == 564 ||
				x == 426 && z == 554 || x == 389 && z == 554 || x == 383 && z == 564 ||
				x == 472 && z == 589 || x == 472 && z == 576 || x == 472 && z == 600 ||
				x == 480 && z == 607 || x == 530 && z == 607 || x == 538 && z == 601 ||
				x == 538 && z == 565 || x == 531 && z == 556 || x == 479 && z == 556) {
				file_w << 1;
			}
			else {
				file_w << 0;
			}
		}
		file_w << endl;
	}
	*/
	file_s3.close();
	cout << "Map Complete !.";
}

void load_map()
{
	cout << "Map Hometown Load..." << endl;
	char h_line[W_WIDTH];
	char* hline;
	ifstream read("Map/hometown/hometown.txt");
	FILE* hometown_t = fopen("Map/hometown/hometown.txt", "r");
	if (!read.is_open()) {
		cout << "map does not exist." << endl;
		return;
	}
	int z = 0;
	while (!feof(hometown_t)) {
		hline = fgets(h_line, W_HEIGHT, hometown_t);
		for (int w = 0; w < W_WIDTH; ++w) {
			hometown[z][w] = h_line[w];
		}
		z++;
	}
	fclose(hometown_t);
	read.close();

	cout << "Map Stage1 Load..." << endl;
	char s1_line[W_WIDTH];
	char* s1line;
	ifstream s1_read("Map/stage1/stage1.txt");
	FILE* stage1_t = fopen("Map/stage1/stage1.txt", "r");
	if (!s1_read.is_open()) {
		cout << "map does not exist." << endl;
		return;
	}
	z = 0;
	while (!feof(stage1_t)) {
		s1line = fgets(s1_line, W_HEIGHT, stage1_t);
		for (int w = 0; w < W_WIDTH; ++w) {
			stage1[z][w] = s1_line[w];
		}
		z++;
	}
	fclose(stage1_t);
	s1_read.close();

	cout << "Map Stage2 Load..." << endl;
	char s2_line[W_WIDTH];
	char* s2line;
	ifstream s2_read("Map/stage2/stage2.txt");
	FILE* stage2_t = fopen("Map/stage2/stage2.txt", "r");
	if (!s2_read.is_open()) {
		cout << "map does not exist." << endl;
		return;
	}
	z = 0;
	while (!feof(stage2_t)) {
		s2line = fgets(s2_line, W_HEIGHT, stage2_t);
		for (int w = 0; w < W_WIDTH; ++w) {
			stage2[z][w] = s2_line[w];
		}
		z++;
	}
	fclose(stage2_t);
	s2_read.close();

	cout << "Map Stage3 Load..." << endl;
	char s3_line[W_WIDTH];
	char* s3line;
	ifstream s3_read("Map/stage3/stage3.txt");
	FILE* stage3_t = fopen("Map/stage3/stage3.txt", "r");
	if (!s3_read.is_open()) {
		cout << "map does not exist." << endl;
		return;
	}
	z = 0;
	while (!feof(stage3_t)) {
		s3line = fgets(s3_line, W_HEIGHT, stage3_t);
		for (int w = 0; w < W_WIDTH; ++w) {
			stage3[z][w] = s3_line[w];
		}
		z++;
	}
	fclose(stage3_t);
	s3_read.close();
	/*
	for (int z = 0; z < W_HEIGHT; ++z) {
		for (int x = 0; x < W_WIDTH; ++x) {
			file >> map[z][x];
		}
	}
	*/

}