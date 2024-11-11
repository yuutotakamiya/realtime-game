<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    public function up(): void
    {
        Schema::create('stage_logs', function (Blueprint $table) {
            $table->id();
            $table->integer('stage_id');//ステージID
            $table->integer('user_id');//ユーザーID
            $table->boolean('result');//完了or失敗
            $table->integer('min_hand_num');//手数
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('stage_logs');
    }
};
