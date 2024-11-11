<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('landstatuses', function (Blueprint $table) {
            $table->id();
            $table->integer('land_id');//島ID
            $table->integer('user_id');//ユーザーのID
            $table->integer('land_block_num');//島でブロックを埋めた数
            $table->timestamps();

            $table->index('user_id');//ユーザーIDをindex化
            $table->index('land_id');//島IDをindex化
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('landstatus');
    }
};
